# Fil rouge : choisir son scénario

Les [thématiques](../../thematiques/README.md) du module sont **indépendantes du scénario**. Le scénario donne le décor dans lequel on applique, thématique après thématique, ce que les [drills](../drills/README.md) et le [chat](../chat/README.md) ont fait comprendre.

| Scénario                           | Décor                                                                  | État                            | Choisissez-le si…                                                                                                                                                           |
|------------------------------------|------------------------------------------------------------------------|---------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [UMDD (Dousville)](umdd/README.md) | une ville de boulangeries-pâtisseries, services partagés par la classe | complet                         | vous voulez le scénario le plus guidé : une page par phase, des services de référence, collaboratif, langage libre                                                                                                |
| [Powercher](powercher/README.md)   | maisons connectées, énergie, « Mère Nature »                           | repris des activités existantes | vous voulez réutiliser le matériel déjà testé ([powercher_v1](../powercher_v1/README.md), [powerdeal](../powerdeal/README.md), [aie_confiance](../aie_confiance/README.md)) |
| [Pedaloland](pedalo/README.md)     | l'industrie du pédalo                                                  | esquisse                        | à réserver à un autre groupe : [Pedaloland.md](../../Pedaloland.md)                                                                                                         |
| [Libre](libre/README.md)           | décidé par la classe                                                   | à construire                    | votre groupe veut inventer son propre scénario, avec le [gabarit](modele/README.md) et le [catalogue de services](services.md)                                              |

## Ce qui ne change pas

Quel que soit le scénario :

- le [contrat de base](../contrats/README.md) (enveloppe, présence, temps, log, battement) ;
- le [labo Docker](../infra/README.md) ;
- les [thématiques](../../thematiques/README.md) et leurs [supports](../../supports/README.md) ;
- les [drills](../drills/README.md) et le [chat](../chat/README.md) ;
- l'[évaluation](../../evaluation/grille.md).

## Ce qui change

Les **noms** des services et leurs règles métier (boulangerie, maison, usine à pédalos…), la racine de topics et les types de messages ajoutés. Voir le [catalogue de services](services.md). La structure est la même : chaque scénario répond aux mêmes questions (qu'est-ce qui s'affiche ? quelle horloge commune ? qui est vivant ? qui paie ? …).


## Contrat de scénario

Un scénario, y compris le [libre](libre/README.md), doit satisfaire, pour chaque thématique, les exigences de sa section « Exigences du fil rouge ». Les services qu'il choisit pour cela sont listés dans le [catalogue de services](services.md). Un [gabarit](modele/README.md) est fourni pour en écrire un nouveau.

| Thématique | Le scénario doit introduire… | Preuve attendue |
|---|---|---|
| [Sérialisation](../../thematiques/01-serialisation.md#exigences-du-fil-rouge) | un type de donnée métier avec un schéma partagé | message valide, trois entrées hostiles ignorées |
| [Temps](../../thematiques/02-temps.md#exigences-du-fil-rouge) | une source d'heure commune | même heure à moins d'une seconde, décalage visible |
| [MQTT](../../thematiques/03-mqtt.md#exigences-du-fil-rouge) | un flux d'état retenu et un flux d'événements | un arrivant tardif voit l'état courant |
| [Microservices](../../thematiques/04-microservices.md#exigences-du-fil-rouge) | deux services indépendants, mort annoncée | nœud tué retiré en moins de 10 s |
| [Idempotence](../../thematiques/05-idempotence.md#exigences-du-fil-rouge) | une opération à effet rejouable | un doublon ne change rien |
| [Transactions](../../thematiques/06-transactions.md#exigences-du-fil-rouge) | une opération à deux participants, tout ou rien | état cohérent après panne |
| [Résilience](../../thematiques/07-resilience.md#exigences-du-fil-rouge) | une dépendance qui peut tomber | retry avec backoff sous panne provoquée |
| [Cluster et P2P](../../thematiques/08-cluster-p2p.md#exigences-du-fil-rouge) | un service répliqué ou un échange direct | perte d'un membre sans perte de service |
| [Sécurité](../../thematiques/09-securite.md#exigences-du-fil-rouge) | un auteur à prouver, une donnée à protéger | usurpation et rejeu refusés |
| [Supervision](../../thematiques/10-supervision.md#exigences-du-fil-rouge) | un état de santé par nœud | silence détecté après le délai attendu |
| [Énergie](../../thematiques/11-energie.md#exigences-du-fil-rouge) | une grandeur à bilanner dans le temps | bilan cohérent sous perte de messages |

Le minimum qu'un scénario contient : un acteur qui fournit le temps, un qui change un état partagé sans risque de doublon ni de demi-succès (paiement, réservation, stock…), un qui est observé, et un qui peut mourir sans tuer les autres.
