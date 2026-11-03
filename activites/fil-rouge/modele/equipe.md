# Exemple de répartition en équipe

**Exemple, à adapter.** Rien ici n'est imposé : c'est une façon de faire qui a l'avantage d'être explicite.

Situation : un groupe de six construit un scénario avec quatre services choisis dans le [catalogue](../services.md) : source de temps, annuaire, paiement, supervision.

## 1. Une personne, un propriétaire par service

| Service | Propriétaire | Relecteur du contrat |
|---|---|---|
| source de temps | A | B |
| annuaire | B | C |
| paiement | C | D |
| supervision | D | A |

E et F n'ont pas de service attitré : l'un écrit le simulateur ou le passage de test, l'autre la passerelle ou le défi. Un propriétaire **répond** de son service (disponibilité, contrat, DoD) mais peut être aidé.

## 2. Se mettre d'accord sur les messages avant de coder

Avant la première ligne de code, chaque propriétaire écrit dans le dossier du scénario :

- les topics (en `{racine}/…`), types et schémas de son service ;
- un exemple de message valide et un invalide ;
- ce qu'il fait des invalides.

Son relecteur lit le contrat en jouant un client : « avec cette seule page, puis-je écrire mon client ? ». Si non, le contrat est corrigé **avant** de coder (voir [contrats écrits par la classe](../../contrats/README.md#contrats-ecrits-par-la-classe)).

## 3. Tester un service sans ses voisins

- On joue le voisin à la main avec les outils du [labo](../../infra/README.md) (`mosquitto_pub`, `mosquitto_sub`).
- On rejoue les exemples valides et invalides du contrat.
- Le prof peut fournir un service de référence en filet de sécurité.

## 4. Un point de vie de l'équipe

Un `hb` par service (voir le [battement](../../contrats/README.md#battement-heartbeat)) et une seule question en début de séance : « tous les services sont-ils vivants ? ». Sinon on répare ensemble avant d'avancer.
