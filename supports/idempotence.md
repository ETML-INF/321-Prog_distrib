# IdemPotence

> **Fallacy :** 1 (le réseau est fiable).  
> **À retenir :** sur un réseau, un message peut arriver **deux fois** (retransmission) ou **pas du tout**. Une opération *idempotente* donne le même résultat qu'on l'applique une ou plusieurs fois : c'est ce qui rend les retransmissions inoffensives.

![idempotence.png](assets/idempotence.png)

## Définition

L'idempotence est une propriété d'une opération qui garantit qu'appliquer cette opération **une ou plusieurs fois** produit le **même** résultat. Dans un contexte de microservices, cela évite les actions répétées lors d'erreurs ou de retransmissions de messages.

| Opération | Idempotente ? |
|---|---|
| `solde = 200` (fixer un état) | **oui** |
| `solde += 50` (ajouter une différence) | **non** |
| supprimer l'élément 123 | oui (déjà supprimé : rien ne change) |
| créer un élément avec un identifiant généré par le serveur | non (un élément de plus à chaque appel) |

Règle pratique : **fixer un état** est idempotent, **ajouter une différence** ne l'est pas. Mais idempotent ne veut pas dire *commutatif* : si l'ordre d'arrivée change, le résultat peut changer ([D05](../activites/drills/D05-idempotence.md)).

## Importance dans un système avec MQTT

MQTT fonctionne en publication/souscription et, en QoS 1, garantit une livraison **au moins une fois** : des messages peuvent être reçus plusieurs fois. Les consommateurs doivent donc traiter les messages de façon idempotente.

<details>
<summary>Quel(s) QOS(s) de MQTT impliquent obligatoirement un traitement de l’idempotence</summary>
QoS 1 (au moins une fois : doublons possibles). Le QoS 2 limite les doublons entre un client et le broker, mais ne protège pas d'un nouvel envoi décidé par l'application (relance après timeout, redémarrage de l'émetteur) : l'idempotence reste nécessaire.
</details>

## Stratégies

1. **Rendre l'opération naturellement idempotente** : fixer un état plutôt qu'ajouter une différence.
2. **Dédupliquer par identifiant.** Chaque message porte un identifiant **unique** (UUID). Le consommateur garde les identifiants déjà traités et ignore ceux qu'il reconnaît.
   - L'émetteur **réutilise le même identifiant** à chaque retransmission d'un même message. S'il en crée un nouveau à chaque essai, le récepteur ne peut plus reconnaître le doublon.
   - La liste des identifiants vus ne doit pas grossir sans fin : on la **purge** en gardant au moins la durée maximale de redélivrance (par exemple 24 h, grâce à l'horodatage du message).
3. **Clé d'idempotence côté client.** Le client génère un identifiant pour *l'intention* (« créer cette commande ») et le joint à la requête ; le serveur refuse ou ignore une seconde requête portant le même identifiant.

## Idempotence avec HTTP PUT et POST pour ajouter un élément dans une base de données

Selon les [spécifications HTTP](https://www.rfc-editor.org/rfc/rfc9110.html) :

| **Aspect** | **POST** | **PUT** |
|---|---|---|
| Idempotence | non idempotent | idempotent |
| Usage principal | créer une nouvelle ressource | créer ou remplacer une ressource à une adresse précise |
| Résultat si répété | plusieurs ressources créées | toujours le même état |
| Identifiant | généré par le serveur | **fourni par le client** (dans l'URL) |

```
POST /api/items            {"name":"item1","value":100}   →  crée un élément, nouvel identifiant à chaque appel
PUT  /api/items/123        {"name":"item1","value":100}   →  crée ou remplace l'élément 123, quel que soit le nombre d'appels
```

- **PUT** exige que le client connaisse l'identifiant (l'URL) de la ressource : la même requête ne peut pas créer de doublon.
- Pour rendre un **POST** sûr, on joint un identifiant unique (corps ou en-tête) que le serveur mémorise ; une seconde requête avec le même identifiant est refusée (code *409 Conflict*) ou renvoie le résultat de la première.
- Variante en deux étapes : le client demande d'abord au serveur un **identifiant unique**, puis crée la ressource avec **PUT** (ou POST avec cet identifiant). Le serveur garde le contrôle des identifiants, et les requêtes répétées ne créent pas de doublons.
- Les autres verbes (GET, DELETE, PATCH, HEAD, OPTIONS…) : voir [l'activité](../activites/idempotence/README.md).

Utilisez **POST** quand le serveur doit contrôler l'identifiant, **PUT** quand le client contrôle la ressource visée.

## Exemple : une enveloppe de message

Les messages d'environnement de la ville (date, nuages, énergie solaire, vent) peuvent être retransmis. Un choix possible : les messages sont emballés dans une **enveloppe** qui sépare la partie « technique » de la partie métier, avec un identifiant unique, l'émetteur, le destinataire, le type de message et le contenu. Un consommateur mémorise les identifiants déjà traités.

<details>
<summary>Quel est le piège de « un UUID est généré à chaque envoi » ?</summary>
Si l'enveloppe est reconstruite à chaque tentative de retransmission, chaque essai porte un nouvel identifiant et la déduplication ne reconnaît rien. L'identifiant doit être fixé à la création du message, pas à son envoi.
</details>

L'enveloppe du contrat de base (`id`, `from`, `to`, `ts`, `type`) suit la même idée : voir le [contrat](../activites/contrats/README.md).

## Conclusion

L'idempotence est un élément clé d'un système résilient (tolérant aux pannes) et fait partie du couteau suisse d'un réseau IoT fiable. Elle se combine avec les **retries** ([Design for failure](design4failure.md)) et les **transactions** ([Transaction](transaction.md)).

![idempotence2.png](assets/idempotence2.png)

## Pour s'exercer

- Drill : [D05](../activites/drills/D05-idempotence.md) · Chat : [C5](../activites/chat/C5-doublons.md) · Activité : [Idempotence en pratique](../activites/idempotence/README.md)
