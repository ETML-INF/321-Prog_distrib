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
| **Strict ou tolérant** | Les payloads du noyau (`presence`, `time`…) interdisent les champs inconnus : un champ en trop les rend invalides | Voir la note sous la règle 2 du [contrat](../activites/contrats/README.md) : la tolérance vaut pour les types inconnus |

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
