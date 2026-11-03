# UMDD · L'annuaire ne doit plus tomber

L'annuaire est **sans état** : c'est le candidat idéal pour tourner en plusieurs exemplaires. Mais « plusieurs exemplaires » pose une question : lequel répond ?

| | |
|---|---|
| Prérequis | [D07](../../drills/D07-retry.md) · [D08](../../drills/D08-cluster.md) · [C6](../../chat/C6-reconnexion.md) |
| Fallacy | 5 (topologie), 6 (un seul administrateur), 1 (fiabilité) |
| Contrat | celui de l'[annuaire](log-annuaire.md), éventuellement amendé |
| Exigence | [Résilience](../../../thematiques/07-resilience.md#exigences-du-fil-rouge) · [Cluster et P2P](../../../thematiques/08-cluster-p2p.md#exigences-du-fil-rouge) |

## Mission

Déployer **quatre instances** de l'annuaire dans des conteneurs séparés (les quatre groupes d'annuaire y contribuent), de sorte qu'à tout moment les clients obtiennent **une** réponse correcte.

Deux familles de solutions, **à choisir et défendre** :

| | Principe | Panne qui le met à mal |
|---|---|---|
| Répartition | toutes les instances sont égales ; un abonnement partagé donne chaque requête à une seule | une instance lente |
| Élection | une seule instance répond, désignée par une règle | partition : deux se croient élues |

## DoD

- Quatre instances tournent ; une BP qui demande reçoit une réponse correcte.
- Je tue l'instance qui répond (`docker kill`) : une autre prend le relais, sans intervention humaine.
- Je tue **trois** instances sur quatre : le service continue.
- Je coupe une instance du broker avec le [proxy](../../infra/README.md) (`timeout`) : quel est le comportement ? Je le démontre.
- Les **clients** (les BP) réessayent avec backoff + jitter pendant la bascule ([C6](../../chat/C6-reconnexion.md)).

## Indices

- Un annuaire qui répond deux fois est-il gênant ? (relisez [D08](../../drills/D08-cluster.md), question 6). Que faut-il à ses clients pour s'en moquer ?
- L'abonnement partagé MQTT 5 (`$share/<groupe>/<topic>`) donne la répartition sans écrire d'élection.
- Orchestrez avec `docker compose`, un service par instance : partez de [`compose.yaml`](../../infra/compose.yaml).
- Documentez le temps de bascule mesuré et ce qui le détermine (keep-alive ? backoff ?).
