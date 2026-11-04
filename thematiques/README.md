# Thématiques

Telles que présentées [ici](https://roadmap.sh/r/embed?id=66ea73d4f34c8868ec042b2c)

Une thématique = un sujet de théorie et tout ce qui permet de l'éprouver. L'**ordre** et le **découpage en séances** ne sont pas décidés ici : voir le [planning](../planning/README.md), qui n'est qu'une suggestion.

## Ce que contient chaque thématique

1. **Théorie** : les [supports](../supports/README.md) correspondants.
2. **[Drill](../activites/drills/README.md)** : un mécanisme éprouvé à la main, avec le résultat attendu à chaque étape (≈ 20 min, sans IA).
3. **[Chat](../activites/chat/README.md)** : une amélioration guidée de **son** chat, dans **le langage de son choix**, vérifiable avec le [labo](../activites/infra/README.md).
4. **[Fil rouge](../activites/fil-rouge/README.md)** : le même mécanisme dans un décor choisi (UMDD, Powercher, Pedaloland), moins guidé, avec des indices.

L'IA est permise à condition de **comprendre** : voir l'[évaluation](../evaluation/grille.md).

## Les thématiques

| Thématique | Services attendus | Fallacies | Supports | Drill | Chat | Fil rouge (UMDD) |
|---|---|---|---|---|---|---|
| [Sérialisation](01-serialisation.md) | Vitrine / état public, Passerelle / adaptateur | 3, 8 | [Sérialisation](../supports/Sérialisation.md) | D01 | C1 | [Vitrine](../activites/fil-rouge/umdd/vitrine.md) |
| [Temps](02-temps.md) | Source de temps, Simulateur d’événements | heure, 2 | [NTP](../supports/ntp.md) | D02 | C2 | [TimeService](../activites/fil-rouge/umdd/time.md) |
| [MQTT](03-mqtt.md) | Source de temps | 1, 2 | [MQTT](../supports/mqtt.md) | D03 | C3 | [TimeService](../activites/fil-rouge/umdd/time.md) |
| [Microservices](04-microservices.md) | Annuaire, Journal | 1, 5, 6 | [Microservice](../supports/microservice.md) | D04 | C4 | [Log et annuaire](../activites/fil-rouge/umdd/log-annuaire.md) |
| [Idempotence](05-idempotence.md) | Paiement | 1 | [Idempotence](../supports/idempotence.md) | D05 | C5 | [Banque et coopérative](../activites/fil-rouge/umdd/banque-coop.md) |
| [Transactions](06-transactions.md) | Échange | 1, 6 | [Transaction](../supports/transaction.md) | D06 | | [Banque et coopérative](../activites/fil-rouge/umdd/banque-coop.md) |
| [Résilience](07-resilience.md) | Stockage d’état, Simulateur d’événements | 1, 2 | [Design for failure](../supports/design4failure.md) | D07 | C6 | [Annuaire en cluster](../activites/fil-rouge/umdd/cluster.md) |
| [Cluster et P2P](08-cluster-p2p.md) | Coordinateur, Annuaire | 5, 6 | [Cluster](../supports/cluster.md) · [P2P](../supports/P2P.md) | D08 | | [Annuaire en cluster](../activites/fil-rouge/umdd/cluster.md) |
| [Sécurité](09-securite.md) | Identité et confiance, Passerelle / adaptateur | 4, 6 | [Sécurité](../supports/securite.md) · [Confidentialité](../supports/confidentialite.md) · [Réseau de confiance](../supports/reseau-confiance.md) | D09 | C7 | [Sécurité](../activites/fil-rouge/umdd/securite.md) |
| [Supervision](10-supervision.md) | Supervision, Journal | 1, 7 | [Monitoring](../supports/monitoring.md) | D10 | C8 | [Supervision](../activites/fil-rouge/umdd/supervision.md) |
| [Énergie](11-energie.md) | Supervision, Simulateur d’événements | 7 | [Puissance et énergie](../supports/Puissance%20et%20énergie.md) | D10 | C8 | [Supervision](../activites/fil-rouge/umdd/supervision.md) |

Chaque thématique précise ce qu’un fil rouge doit y introduire (section « Exigences du fil rouge ») : voir le [contrat de scénario](../activites/fil-rouge/README.md#contrat-de-scenario). Un fil rouge [libre](../activites/fil-rouge/libre/README.md) peut être construit par la classe.

## Les 8 fallacies, fil rouge du module

[Les 8 fallacies](../supports/fallacies.md) ([slides](../slides/fallacies/)) sont la grille de lecture de **toutes** les thématiques : chaque affirmation de l'entrepreneur du scénario en est une. Lisez chaque énoncé en demandant « de quelle croyance dépend-il ? ».

## Outils transversaux

- [Labo Docker](../activites/infra/README.md) : broker + proxy de pannes sur un seul hôte.
- [Contrat de base](../activites/contrats/README.md) : le seul document commun, quel que soit votre langage.
- [Compétences visées](../evaluation/matrice%20des%20compétences.md).
