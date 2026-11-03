# UMDD · Journal et annuaire

Deux premiers microservices : un service qui **écoute** (Log) et un service qui **tient le registre** (Annuaire). Premier travail de groupes, premiers contrats à écrire ensemble.

|           |                                                                                    |
|-----------|------------------------------------------------------------------------------------|
| Prérequis | [D04](../../drills/D04-mort-noeud.md) · [C4](../../chat/C4-mort.md)                |
| Fallacy   | 1 (fiabilité), 5 (topologie), 6 (plusieurs administrateurs)                        |
| Contrat   | [log](../../contrats/log.schema.json) fourni ; **annuaire à écrire par la classe** |
| Exigence | [Microservices](../../../thematiques/04-microservices.md#exigences-du-fil-rouge) |

## A. LogService (écriture seule)

Chaque BP décide des événements qu'elle journalise et les publie sur `dousville/log/<bp>`.

**Mission :** réaliser votre client `logger`, application console :

```
logger --help
usage : logger [--help] [--version] [--broker=<adresse>] [--source=<nom>]
  broker   adresse du broker. Par défaut : le broker de la classe
  source   BP à suivre. Par défaut : toutes
```

**DoD :**

- `logger --version` affiche `Dousville logger … v1.0` avec **vos initiales** à la place des points.
- Ma BP journalise `Hello from …` au démarrage, `Hey from …` périodiquement, `Bye from …` à l'arrêt.
- Je tue ma BP brutalement : je vois `… died!` dans son log (LWT, [C4](../../chat/C4-mort.md)).
- Je montre le log d'**une** BP seulement alors que les autres journalisent, puis le log de **toutes** en même temps.

*Question :* à quelle période faut-il le `Hey` ? Qu'est-ce que 30 BP × 1 message/s coûtent ?

## B. Annuaire (liste des BP ouvertes)

Un service **sans état** qui sait quelles BP sont ouvertes.

**À concevoir en groupe de 3 puis à faire valider par la classe** (pull request sur `activites/contrats`) :

- les topics et messages : `whoisopen` (demande), `iamopen`, `iamclosed`, `open` (liste diffusée) ;
- qui parle à qui (global ou adressé) ;
- que fait l'annuaire quand il redémarre ? Que fait une BP qui l'entend demander ?
- comment l'annuaire sait qu'une BP est morte (LWT).

**DoD :**

- Je démarre une BP sur le poste A : elle s'affiche seule. Une deuxième sur le poste B : les deux s'affichent, partout.
- J'arrête une BP **proprement**, elle disparaît de tous les postes.
- J'arrête une BP **brutalement**, elle disparaît de tous les postes (délai à mesurer et à justifier).
- J'envoie deux fois la même annonce `iamopen` : la liste ne contient la BP qu'**une fois** ([D05](../../drills/D05-idempotence.md)).

## Indices

- Pendant le développement utilisez un préfixe de topic privé (ex. `dousville-dev1/…`) pour ne pas polluer la classe.
- La liste `open` change à chaque ouverture/fermeture : réfléchissez à l'idempotence (renvoyer la liste complète plutôt qu'un « delta »).
- Un groupe sera retenu pour tenir l'annuaire officiel (dockerisé), les autres deviennent les instances de secours du [cluster](cluster.md).
- Filet de sécurité : annuaire et LogService de référence côté prof.

## Pannes à provoquer

- Tuez l'annuaire pendant qu'une BP s'ouvre. Que se passe-t-il quand l'annuaire revient ?
- Coupez le [proxy](../../infra/README.md) 10 s : votre annuaire est-il toujours cohérent ?
