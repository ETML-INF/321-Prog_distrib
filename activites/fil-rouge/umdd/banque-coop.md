# UMDD · Banque, coopérative et Fair Trade

De l'argent et un stock : la première fois que « ça marche presque » coûte vraiment.

| | |
|---|---|
| Prérequis | [D05](../../drills/D05-idempotence.md) · [D06](../../drills/D06-transaction.md) · [C5](../../chat/C5-doublons.md) |
| Fallacy | 1 (fiabilité), 6 (plusieurs administrateurs) |
| Contrat | **banque et coopérative à écrire par la classe** (voir [contrats](../../contrats/README.md#contrats-ecrits-par-la-classe)) |
| Exigence | [Idempotence](../../../thematiques/05-idempotence.md#exigences-du-fil-rouge) · [Transactions](../../../thematiques/06-transactions.md#exigences-du-fil-rouge) |

## A. La coopérative (entrepôt)

Chaque BP peut : **livrer** des spécialités, demander l'**inventaire**, **prélever** des spécialités.

DoD :
- j'envoie 10 exemplaires de ma spécialité ; je les retrouve dans l'inventaire ;
- je prélève une spécialité d'une autre BP ;
- le service est dockerisé ; le mode d'emploi est publié pour la classe.

## B. La banque

Un compte courant par BP, initialisé au démarrage. Une BP peut : **consulter son solde**, **transférer** vers un autre compte, être **notifiée** d'un versement.

DoD :
- ma BP affiche l'état de mon compte, celle de mon collègue le sien ;
- un transfert d'un montant inférieur au solde diminue mon compte et augmente le sien ;
- **un transfert envoyé deux fois (même `id`) n'est exécuté qu'une fois** ;
- le service est dockerisé ; le mode d'emploi est publié.

## C. Fair Trade (transaction)

La coopérative a un compte. À chaque livraison, elle **paie** l'artisan. Si son compte est insuffisant, l'artisan **récupère** sa livraison.

DoD :
- livraison payée : stock augmenté, compte de l'artisan crédité ;
- coop sans fonds : livraison refusée, stock inchangé, artisan non débité ni crédité ;
- **la banque plante entre l'exécution et la réponse** (`docker compose pause` sur son conteneur) : à son retour, il n'y a ni double paiement ni livraison perdue.

## Indices

- Relisez vos dessins de [D06](../../drills/D06-transaction.md) : quel est l'état « en cours » de la transaction ? Où est-il stocké ?
- Les contrats (topics, messages, erreurs) sont à rédiger **avant** de coder. Un service qui ne se lit pas dans son contrat est inutilisable.
- Pour tester sans attendre un camarade : jouez le client avec `mosquitto_pub`/`sub` ([infra](../../infra/README.md)).
- Filet de sécurité : banque et coopérative de référence côté prof, pour vos tests de pannes.
