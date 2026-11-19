# Microservices

> **Fallacies :** 2 (latence), 5 (topologie), 6 (plusieurs administrateurs), 8 (homogénéité).  
> **À retenir :** un microservice = une fonction, une équipe, un **contrat**. Les services ne partagent ni mémoire ni langage : ils communiquent par messages, et chacun peut tomber.

![microservice.jpeg](assets/microservice.jpeg)

## Qu'est-ce qu'un microservice ?

Une architecture qui décompose le système en plusieurs services **indépendants et autonomes**. Chaque service est responsable d'une fonctionnalité précise et peut être développé, testé et déployé séparément. On obtient des applications plus flexibles, évolutives et faciles à maintenir, au prix d'un réseau entre les parties.

## Communication réseau entre microservices

| Moyen | Garantie | Typique |
|---|---|---|
| **UDP** | pas de garantie de livraison, rapide | jeux en ligne, vidéo en direct |
| **TCP** | livraison fiable, dans l'ordre | transfert de fichiers, transactions bancaires |
| **HTTP** | requête/réponse sur TCP | API web : un service en appelle un autre et attend |
| **Bus de messages** | asynchrone, publication/souscription | RabbitMQ, Kafka, MQTT : les services publient, les autres s'abonnent |

**HTTP, synchrone.** Le service *utilisateurs* envoie une requête `POST /api/orders/checkout` (corps JSON : utilisateur, produit, quantité) au service *commandes*, qui répond avec un code et un message (`{"success": true}`). Simple, mais l'appelant **attend** : si le voisin est lent ou mort, il l'est aussi.

**Bus de messages, asynchrone.** Le service *utilisateurs* publie un événement (par exemple `user_created`) :

```json
{"event":"user_created","data":{"userId":12345,"email":"john.doe@example.com"}}
```

Le service *notifications*, abonné, envoie la notification quand il le peut. Avantages : pas d'attente, scalabilité (on ajoute ou retire des services sans toucher aux autres), résilience (les messages peuvent être conservés si un service est en panne). Voir [MQTT](mqtt.md). *(Bus de messages revu en détail plus tard dans le module.)*

## Un contrat par service

Comme chaque service a son équipe, ce qui les relie est un **contrat** : topics ou routes, forme des messages, erreurs. Un contrat ambigu rend le service inutilisable. Le format devient une affaire de langage commun ([Sérialisation](Sérialisation.md)) et non de code partagé : chaque équipe peut choisir son langage (fallacy 8). Dans ce module : [contrat de base](../activites/contrats/README.md).

## Valider à la frontière

Un contrat écrit ne sert à rien s'il n'est pas **vérifié**. Le broker transporte des octets : il ne connaît pas le contrat. La garde se tient donc **chez le récepteur**, à la réception, avant tout usage du payload. Jamais « on suppose que l'autre équipe a bien fait » (fallacy 6 : plusieurs administrateurs).

**Que faire d'un message invalide ?** L'**ignorer**, le **logger**, l'ajouter au compteur `bad`, et ne **jamais planter** (règle 1 du [contrat de base](../activites/contrats/README.md)). Valider se fait en deux temps : le message entier contre `envelope`, puis son `payload` contre le schéma correspondant à `type`.

### Trois pièges

| Piège | Ce qui arrive | Parade |
|---|---|---|
| **`format` n'est pas assertif par défaut** | `"format":"uuid"` ou `"date-time"` acceptent n'importe quelle chaîne : `id:"nope"` est déclaré valide | Activer l'assertion de format dans la bibliothèque (Ajv : `ajv-formats` ; JsonSchema.Net : `RequireFormatValidation`) |
| **Le testament est figé à la connexion** | Le LWT est préparé avant la panne : son `id` et son `ts` sont ceux de la connexion. Valide, mais périmé | Ne pas se fier au `ts` d'un testament ; ne pas dédoublonner sur son `id` |
| **Strict ou tolérant** | Les payloads du noyau (`presence`, `time`…) interdisent les champs inconnus (`additionalProperties: false`) : un champ en trop les rend invalides, et **chaque évolution devient une rupture** | Schémas tolérants pour vos propres types, voir [Faire évoluer un contrat](#faire-evoluer-un-contrat) |

## Faire évoluer un contrat

Un contrat vit plus longtemps que le code : tôt ou tard il faut y ajouter quelque chose, alors que les autres équipes n'ont pas encore mis à jour. Ce qui rend l'évolution facile ou douloureuse se décide **dès l'écriture du schéma**.

### `additionalProperties: false`, le piège

Ce mot-clé dit « tout champ non listé est une erreur ». C'est strict, donc rassurant, et c'est aussi ce qui **casse les évolutions** :

```json
{"type":"object","required":["state"],"additionalProperties":false,
 "properties":{"state":{"enum":["open","closed","died"]}}}
```

L'émetteur ajoute un champ optionnel `kind` à `presence` : `{"state":"open","kind":"service"}`. Tout lecteur qui valide avec ce schéma **refuse** le message (il l'ignore, le logue et l'ajoute à `bad`, comme le demande la règle 1). Un champ pourtant **inoffensif** met hors service la moitié du réseau, et l'ordre de déploiement devient obligatoire : *tous les lecteurs d'abord, puis le premier émetteur*. Avec plusieurs administrateurs (fallacy 6), personne ne peut l'imposer.

| | Schéma strict (`false`) | Schéma tolérant (par défaut) |
|---|---|---|
| Champ en plus | message refusé | ignoré |
| Ajouter un champ optionnel | **rupture**, il faut une nouvelle version | sans conséquence |
| Faute de frappe dans un nom de champ | détectée | passe inaperçue |
| Ordre de déploiement | lecteurs avant émetteurs, obligatoire | libre |

À retenir : être **strict en émission, tolérant en réception**. On valide ce dont on a besoin (`required`, types, valeurs) et on **ignore le reste**.

### Ce qui change la version, ce qui ne la change pas

`v` est la **version majeure** du contrat et choisit les schémas qui valident le message.

- **Additif, `v` inchangée** : champ optionnel, nouveau `type`, nouveau topic. Possible seulement si les lecteurs sont tolérants.
- **Cassant, nouvelle `v`** : retirer ou renommer un champ, changer son sens, rendre obligatoire un champ optionnel.

### Introduire une nouvelle version : élargir, basculer, resserrer

1. **Élargir** : les lecteurs acceptent `N` et `N+1` ; les émetteurs publient encore `N`. Rien ne change sur le réseau.
2. **Basculer** : quand tous les lecteurs acceptent `N+1`, les émetteurs publient `N+1`.
3. **Resserrer** : annoncer une **date de retrait** pour `N`. D'ici là, un message `N` est traité en **mode dégradé** (sans les nouveautés) et son émetteur est averti, une fois. À la date, `N` est refusée.

Pourquoi ne pas publier les deux versions en même temps ? Sur un bus MQTT, un topic **retenu** ne garde qu'un message et un **testament** (LWT) est unique : la dernière écriture efface l'autre. On élargit donc les lecteurs d'abord.

### Cas réel : El Choco, `presence` et `kind`

Le HUD affichait le service `time` comme une famille : une présence `open` ne dit pas si l'émetteur est une chocolaterie ou un service. La solution propre est un champ `kind` dans le payload de `presence` :

```json
{"state":"open","kind":"service"}
```

Mais le schéma `presence` de la version 1 est strict : l'ajouter, c'était **casser** tous les lecteurs. Le contrat d'El Choco est donc passé en **v2** :

| `v` | Statut | Ce que fait un lecteur |
|---|---|---|
| 2 | en vigueur | schémas **tolérants** (aucun `additionalProperties: false`), `presence` gagne `kind` (`service` ou `family`, défaut `family`) ; les champs inconnus sont ignorés |
| 1 | dépréciée, **retirée le 12.12.2026** | lue en **mode dégradé** : validée contre les anciens schémas, sans `kind` (le HUD se rabat sur une liste d'identifiants connus), un avertissement par émetteur ; refusée à la date de retrait |

Les émetteurs publient la v2 ; un seul endroit du code (un tableau `version → statut`) décide de ce que chaque lecteur accepte, de ce qu'il dégrade et de ce qu'il refuse.

Pour essayer sans code : [JSONLint — JSON Schema](https://jsonlint.com/json-schema){target="_blank"} (schéma à gauche, données à droite ; il ne gère pas `format`, il n'accepte donc pas `envelope`).

À pratiquer : [D04](../activites/drills/D04-mort-noeud.md) (le testament est-il valide ?) puis [C4](../activites/chat/C4-mort.md) (étape « message empoisonné »).

## Avantages et inconvénients

| | Avantage | Inconvénient (et fallacy associée) |
|---|---|---|
| **Charge** | **Scalabilité** : on met à l'échelle chaque service séparément | **Complexité** : plus d'éléments à mettre en place et à gérer |
| **Pannes** | **Résilience** : un service tombe, les autres continuent | **Latence** (2) : chaque appel traverse le réseau |
| **Évolution** | **Évolutivité** : développement et déploiement indépendants | **Données** : réparties entre services, cohérence à assurer ([Transaction](transaction.md)) |
| **Technologies** | **Flexibilité** : un langage adapté à chaque service | **Sécurité** (4) : chaque lien est une surface d'attaque |

La résilience n'est pas automatique : elle exige qu'un service sache qu'un voisin est mort (LWT, [D04](../activites/drills/D04-mort-noeud.md)), réessaye intelligemment ([Design for failure](design4failure.md)) et accepte les doublons ([Idempotence](idempotence.md)).

## Pour s'exercer

- Drill : [D04](../activites/drills/D04-mort-noeud.md) · Chat : [C4](../activites/chat/C4-mort.md) · Fil rouge : [services](../activites/fil-rouge/services.md)
