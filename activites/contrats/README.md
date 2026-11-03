# Contrat de base

Seule source de vérité entre les participants. **Aucun code n'est fourni**, quel que soit le langage : vous lisez le contrat, vous implémentez.  
Le contrat grandit au fil des [thématiques](../../thematiques/README.md). Il est **partagé par tous les fils rouges** : chaque [scénario](../fil-rouge/README.md) y ajoute sa racine de topics et ses propres types de messages.

## Noyau de protocole

### Transport

| Élément | Valeur |
|---|---|
| Broker (labo local) | `localhost:1883` direct, `localhost:1884` via proxy, voir [infra](../infra/README.md) |
| Broker (classe) | `mqtt.blue.section-inf.ch:1883` |
| Encodage | UTF-8, JSON ; un message MQTT = une enveloppe |
| Racine des topics | `{racine}/` : `chat/` pour le [chat](../chat/README.md), `lab/` pour les [drills](../drills/README.md), la racine du scénario pour un [fil rouge](../fil-rouge/README.md) |

### Topics du noyau

| Topic | `type` | Thématique | QoS | Retain | Rôle |
|---|---|---|---|---|---|
| `{racine}/presence/<nœud>` | `presence` | [MQTT](../../thematiques/03-mqtt.md) | 1 | oui | ouvert, fermé ou mort (LWT) |
| `{racine}/time` | `time` | [Temps](../../thematiques/02-temps.md) | 0 | non | heure commune |
| `{racine}/log/<nœud>` | `log` | [Microservices](../../thematiques/04-microservices.md) | 0 | non | journal d'un nœud |

Le [chat](../chat/README.md) y ajoute `chat/all` et `chat/dm/<destinataire>` (type `chat`, QoS 0 et 1, non retenus).

### Enveloppe

Tout message est une enveloppe ([schéma](envelope.schema.json)) :

| Champ | Rôle | Exploité en |
|---|---|---|
| `v` | version du contrat (`1`) | Sérialisation (évolution) |
| `id` | UUID unique du message | Idempotence |
| `from` | nom du nœud émetteur | Sécurité (confiance) |
| `to` | nom d'un nœud destinataire, ou `*` pour tous | adressage |
| `ts` | UTC, ISO 8601 | Temps |
| `type` | nom du schéma du `payload` (minuscules, chiffres, tirets) | Sérialisation (validation) |
| `payload` | contenu, selon `type` | |

Exemple :

```json
{"v":1,"id":"0b5f5c0e-3a52-4c58-9b0e-5a3f2b1a9c11","from":"alice","to":"*",
 "ts":"2026-11-02T08:15:30Z","type":"chat","payload":{"text":"salut"}}
```

### Règles

1. Un message qui ne valide pas contre son schéma est **ignoré et loggé**. Il ne fait jamais planter un service.
2. On est **strict en émission, tolérant en réception** : un champ inconnu dans un payload est ignoré, un champ obligatoire absent rend le message invalide.
3. Un nœud ne publie que sous son propre nom (`from`), jusqu'à ce que la thématique Sécurité nous donne le moyen de le prouver.
4. Un `type` pour lequel le nœud n'a pas de schéma est **ignoré et loggé**, comme un message invalide : le `type` est extensible, et sur un broker partagé vous recevrez des types d'autres scénarios.

### Battement (heartbeat)

Un nœud peut dire qu'il est vivant en publiant un message `log` de niveau `info` dont le texte est :

```
hb <clé>=<entier> [<clé>=<entier> …]
```

Exemple : `hb sent=12 recv=40 bad=2 dup=1 reco=0`.

- Topic `{racine}/log/<nœud>`, QoS 0, non retenu.
- Les valeurs sont des compteurs cumulés depuis le démarrage, entiers ≥ 0. Clés du chat : `sent`, `recv`, `bad`, `dup`, `reco`. Un scénario peut en définir d'autres.
- Un moniteur ne se fie qu'à l'**arrivée** des `hb` : un nœud est silencieux après plus de `3 × N` secondes sans `hb` (`N` = période annoncée).
- Un `hb` mal formé est ignoré ; il ne compte pas dans `bad` (qui ne compte que les messages entrants invalides).

### Schémas du noyau

[envelope](envelope.schema.json) · [chat](chat.schema.json) · [presence](presence.schema.json) · [time](time.schema.json) · [log](log.schema.json)

Valider sans écrire de code : n'importe quel validateur JSON Schema (draft 2020-12), en ligne ou en bibliothèque de votre langage.

## Extension par scénario

Un scénario déclare, dans sa page d'accueil :

1. sa **racine de topics** (`{racine}/…`) ;
2. ses **types de messages** supplémentaires et leurs schémas (par exemple un schéma de fiche de commerce, placé dans le dossier du scénario) ;
3. son **vocabulaire** (nœud = commerce, maison, usine…).

Un scénario qui impose un protocole externe (voir la catégorie [passerelle](../fil-rouge/services.md)) fait traduire par ses nœuds vers et depuis ce contrat de base.

## Contrats écrits par la classe

Les contrats des services propres au scénario n'ont **pas** de schéma fourni. Chaque groupe de service :

1. propose ses topics, ses `type` et ses schémas dans une pull request sur le dossier du scénario ;
2. les autres groupes la relisent (un contrat ambigu = un service inutilisable) ;
3. le contrat est fusionné **avant** que les groupes clients ne codent.

Le prof peut jouer le client d'un service pour vérifier qu'un contrat est suffisant.
