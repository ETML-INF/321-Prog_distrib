# UMDD · Le défi de la classe

La ville fonctionne. On la confronte à un **scénario réel**, choisi par vote.

|           |                                                       |
|-----------|-------------------------------------------------------|
| Prérequis | tous les services de la ville                                               |
| Fallacy   | à identifier par l'équipe (c'est le premier livrable) |
| Contrat   | à amender selon le défi                               |
| Exigence | [contrat de scénario](../README.md#contrat-de-scenario) : toutes |

## 1. Le vote

Le prof présente trois propositions. Chacune se justifie par les fallacies qu'elle fait ressurgir. Chaque étudiant vote ; la proposition retenue devient le défi.

| Proposition | Décor | Fallacies en jeu |
|---|---|---|
| Four connecté | capteurs de température dans les fours, alerte de surchauffe, appareils sur batterie | 7 (énergie), 1 (perte), 3 (débit) |
| Livraison | livreurs mobiles, perte de réseau en déplacement, reprise sans doublons | 1, 2, 5 |
| Black Friday | pic de commandes, faillite de la banque, files d'attente | 2, 1, 6 |

*(Les trois propositions sont des exemples : le prof peut les remplacer, la classe peut en proposer.)*

## 2. Avant de coder (obligatoire)

Chaque équipe remet une page :

- les **fallacies** que le défi fait ressurgir, avec **un exemple de panne** pour chacune ;
- le **contrat** à ajouter ou amender ;
- les **tests de panne** prévus avec le [labo](../../infra/README.md).

## 3. Réalisation

Implémentez le défi sur la ville. Le prof circule en **provocateur** : il casse quelque chose (coupe le proxy, tue un service, fige le broker). L'équipe doit montrer la ville se rétablir.

## 4. Et après ?

La ville et ce défi servent de terrain à l'[évaluation](../../../evaluation/grille.md) : une fonctionnalité sur la ville (piste *feature*) ou un code défectueux à réparer (piste *code*).
