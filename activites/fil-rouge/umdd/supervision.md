# UMDD · Surveiller la ville

Pour que chaque BP mène son activité sereinement, une entité dédiée surveille la disponibilité de **tous** les microservices : elle détecte les pannes et alerte l'équipe de maintenance.

| | |
|---|---|
| Prérequis | [D10](../../drills/D10-monitoring-energie.md) · [C8](../../chat/C8-sante.md) |
| Fallacy | 7 (coût), 1 (fiabilité) |
| Contrat | battement : voir le [contrat de base](../../contrats/README.md#battement-heartbeat) ; seules les alertes (`dousville/alerts`) sont à définir par le groupe Supervision |
| Exigence | [Supervision](../../../thematiques/10-supervision.md#exigences-du-fil-rouge) · [Énergie](../../../thematiques/11-energie.md#exigences-du-fil-rouge) |

## Mission

1. Chaque service (Time, Log, Annuaire, Banque, Coop) publie un **battement** (`hb`) à une période justifiée par un calcul ([C8](../../chat/C8-sante.md)).
2. Le **Moniteur** écoute, tient l'état de chaque service (`ok`, `suspect`, `en panne`) et son historique.
3. Une panne déclenche une **alerte** (message sur un topic d'alerte, trace console, ou autre canal de votre choix).
4. Le Moniteur distingue une **panne transitoire** d'une panne **permanente** et ne spamme pas l'alerte.
5. (Au choix) un petit tableau de bord : terminal ou page web.

## DoD

- Je tue chaque service l'un après l'autre : le Moniteur le détecte et alerte dans un délai que je peux expliquer.
- Je relance un service : l'alerte se **clôture** et la durée de panne est consignée.
- Le Moniteur lui-même meurt : comment le sait-on ? (au moins une réponse écrite)
- Je calcule le trafic total de supervision par heure pour 30 BP et le justifie.

## Indices

- Sondage (*polling*) ou événementiel ? Voir [Monitoring](../../../supports/monitoring.md) : chaque choix a un coût et une ambiguïté.
- Un silence peut signifier « mort » ou « rien à dire ». Qu'est-ce qui lève l'ambiguïté ?
- Les compteurs de [C8](../../chat/C8-sante.md) (doublons, invalides, reconnexions) sont la matière première de vos alertes.
- Une fois la ville surveillée, la classe choisit son [défi](defi.md).
