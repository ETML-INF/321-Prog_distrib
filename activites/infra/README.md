# Laboratoire (Docker)

Un hôte, un broker MQTT, un réseau que **vous** pouvez dégrader. Sert aux [drills](../drills/README.md), au [chat](../chat/README.md) et au [fil rouge](../fil-rouge/README.md).

## Télécharger

Placez ces trois fichiers dans le même dossier (`activites/infra`) :

- [compose.yaml](./compose.yaml) : le laboratoire (broker + proxy)
- [mosquitto.conf](./mosquitto.conf) : configuration du broker
- [toxiproxy.json](./toxiproxy.json) : définition du proxy de pannes

## Démarrer

```bash
cd activites/infra
docker compose up -d          # broker + proxy de pannes
docker compose ps             # les deux doivent être "running"
docker compose down           # tout arrêter (-v pour effacer l'état du broker)
```

| Port hôte | Quoi                                       | Quand l'utiliser              |
|-----------|--------------------------------------------|-------------------------------|
| `1883`    | broker, accès **direct**                   | réseau « parfait »            |
| `1884`    | broker, accès **via le proxy** (Toxiproxy) | réseau que l'on peut dégrader |
| `8474`    | API Toxiproxy                              | scripts de panne              |

Outils de temps (D02, fil rouge *Temps*) : `docker compose --profile temps up -d`, voir [Temps](#temps-ntp-local-et-fausse-horloge).

Variante RabbitMQ : `docker compose --profile rabbitmq up -d`. MQTT sur le port `1885`, utilisateur `ict`, mot de passe `321`. Même contrat, autre broker : bon test d'hétérogénéité.

## Outils : aucun à installer

Les clients `mosquitto_pub` / `mosquitto_sub` sont dans le conteneur du broker. Ouvrez **plusieurs terminaux**, un par rôle (Alice, Bob, observateur).

```bash
# observateur : tout ce qui passe
docker compose exec broker mosquitto_sub -t 'lab/#' -v

# publier
docker compose exec broker mosquitto_pub -t lab/test -m 'salut'

# passer par le proxy (depuis le conteneur : hôte "toxiproxy", port 1884)
docker compose exec broker mosquitto_pub -h toxiproxy -p 1884 -t lab/test -m 'salut'
```

Options utiles : `-q 0|1|2` (QoS), `-r` (retain), `-i <id>` (identifiant client), `-c` (session persistante), `-k <s>` (keep-alive, minimum 5), `-W <s>` (abandonner après), `-C <n>` (s'arrêter après n messages), `--will-topic/--will-payload` (LWT).

> Vos propres programmes (quel que soit leur langage) se connectent à `localhost:1883` ou `localhost:1884` depuis l'hôte.  
> Chaque `docker compose exec` coûte 1 à 2 s : les durées mesurées au terminal sont approximatives.

## Temps : NTP local et fausse horloge

Le profil `temps` ajoute deux conteneurs, construits à partir de la même image (`chrony` + `faketime`, construite au premier démarrage, ≈ 1 min) :

| Service | Rôle |
|---|---|
| `ntp` | serveur NTP du labo, joignable sous le nom `ntp` depuis les autres conteneurs. Il sert l'heure de la machine hôte |
| `horloge` | poste de travail : `docker compose exec horloge bash` |

```bash
docker compose --profile temps up -d
docker compose exec horloge chronyd -Q "server ntp iburst"              # mesurer l'écart, sans corriger
docker compose exec horloge chronyd -Q "server ch.pool.ntp.org iburst"  # idem, serveur distant (UDP 123)
docker compose exec horloge faketime -f "+30s" date -u                  # une horloge 30 s en avance
docker compose --profile temps down
```

> Tous les conteneurs lisent **l'horloge de l'hôte** et ne peuvent pas la modifier (`date -s` est refusé). Un écart entre conteneurs se **simule** : `faketime` pour un outil, `--clock-skew` pour votre programme.

## Provoquer des situations dégradées

Chaque ligne s'applique **sur le proxy `mqtt`**. N'affecte que les clients connectés au port `1884`.

| Situation                                                   | Fallacy | Activer                                                                                  | Annuler                                        |
|-------------------------------------------------------------|---------|------------------------------------------------------------------------------------------|------------------------------------------------|
| Latence de 2 s                                              | 2       | `docker compose exec toxiproxy /toxiproxy-cli toxic add -t latency -a latency=2000 mqtt` | `… toxic remove -n latency_downstream mqtt`    |
| Débit limité (5 Ko/s)                                       | 3       | `… toxic add -t bandwidth -a rate=5 mqtt`                                                | `… toxic remove -n bandwidth_downstream mqtt`  |
| Coupure franche (connexion réinitialisée)                   | 1       | `… toxic add -t reset_peer -a timeout=0 mqtt`                                            | `… toxic remove -n reset_peer_downstream mqtt` |
| **Mort silencieuse** (plus rien ne passe, rien ne se ferme) | 1       | `… toxic add -t timeout -a timeout=0 mqtt`                                               | `… toxic remove -n timeout_downstream mqtt`    |
| Proxy coupé / rétabli                                       | 1, 5    | `… toggle mqtt`                                                                          | `… toggle mqtt`                                |
| Voir l'état du proxy                                        |         | `… inspect mqtt`                                                                         |                                                |
| Tout remettre à zéro                                        |         | `docker compose restart toxiproxy`                                                       |                                                |

(`…` = `docker compose exec toxiproxy /toxiproxy-cli`)

Situations qui ne passent pas par le proxy :

| Situation                   | Fallacy | Commande                                                                                         |
|-----------------------------|---------|--------------------------------------------------------------------------------------------------|
| Le broker disparaît         | 1, 5    | `docker compose stop broker` puis `docker compose start broker`                                  |
| Le broker se fige           | 1, 2    | `docker compose pause broker` puis `unpause broker`                                              |
| Un client meurt brutalement | 1       | `kill -9 <pid>` · Task Manager · `docker kill <conteneur>`                                       |
| Réseau hétérogène           | 8       | un client dans un autre langage ; `--profile rabbitmq`                                           |
| Horloge décalée             | heure   | outils : `faketime` (profil [temps](#temps-ntp-local-et-fausse-horloge)) ; votre client : option `--clock-skew <s>` (voir [C2](../chat/C2-ordre.md)) |

## Dépannage

- `docker compose ps` vide : le démon Docker est-il démarré ?
- Un message « retained » vous gêne : `docker compose exec broker mosquitto_pub -t <topic> -r -n` l'efface.
- État du broker corrompu par vos tests : `docker compose down -v` puis `up -d`.
- Port 1883 ou 1884 déjà pris : un ancien laboratoire (projet Compose d'un autre nom, le projet s'appelle maintenant `lab`) tourne encore. `docker compose ls` le montre, `docker compose -p <ancien-nom> down` l'arrête.
