# D03 — MQTT : QoS, session, retain

| | |
|---|---|
| Thématique | [MQTT](../../thematiques/03-mqtt.md) |
| Fallacy | 1 (fiabilité), 2 (latence) |
| Durée | 25 min |
| Matériel | [labo Docker](../infra/README.md) démarré, 2 terminaux |
| Théorie | [MQTT](../../supports/mqtt.md) : *QoS*, *Session persistante*, *Retained Messages* |

## But
Prédire puis vérifier qui reçoit quoi quand un abonné est absent.

Notation : `T1` = terminal de Bob, `T2` = terminal d'Alice. Toutes les commandes se lancent depuis `activites/infra`. `docker compose exec broker` est abrégé en `DC`.

## 1. Prédire (5 min, avant de toucher au clavier)

Alice publie **pendant que Bob est déconnecté**. Complétez avec *reçoit / ne reçoit pas* :

| Cas | QoS Alice | QoS Bob | Session Bob | Retain | Bob reçoit en revenant ? |
|---|---|---|---|---|---|
| A | 0 | 0 | persistante | non | ? |
| B | 1 | 1 | persistante | non | ? |
| C | 1 | 1 | **propre** (non persistante) | non | ? |
| D | 0 | 0 | propre | **oui** | ? |

## 2. Vérifier

### Cas A et B (session persistante)

Bob s'abonne **une fois** pour que le broker garde sa session (`-c` = session persistante, `-i bob` = identifiant stable) puis se déconnecte (`-W 2` : abandonne après 2 s).

1. T1 : `DC mosquitto_sub -i bob -c -q 0 -t lab/a -W 2`  
   → `Timed out` après 2 s : normal, personne n'a publié.
2. T2 : `DC mosquitto_pub -q 0 -t lab/a -m un`
3. T1 : `DC mosquitto_sub -i bob -c -q 0 -t lab/a -W 3 -v`  
   → **rien** (`Timed out`) : cas A, le message QoS 0 est perdu.

Même chose en QoS 1 sur un autre topic (`lab/b`) :

4. T1 : `DC mosquitto_sub -i bob -c -q 1 -t lab/b -W 2`
5. T2 : `DC mosquitto_pub -q 1 -t lab/b -m deux`
6. T1 : `DC mosquitto_sub -i bob -c -q 1 -t lab/b -C 1 -W 3 -v`  
   → `lab/b deux` : cas B, le broker l'a gardé pour Bob.

### Cas C (session propre)

7. Refaites 4 à 6 sur `lab/c` **en retirant `-c`** partout.  
   → rien reçu : sans session persistante le broker n'a pas retenu d'abonnement à servir.

### Cas D (retain)

8. T2 : `DC mosquitto_pub -q 0 -r -t lab/d -m trois`
9. T1 (Bob arrive *après*, session propre) : `DC mosquitto_sub -t lab/d -C 1 -W 3 -v`  
   → `lab/d trois` **immédiatement**.
10. Effacez le message retenu : `DC mosquitto_pub -r -n -t lab/d`

## 3. Dégrader le réseau (fallacy 2)

11. Activez 2 s de latence sur le proxy : `docker compose exec toxiproxy /toxiproxy-cli toxic add -t latency -a latency=2000 mqtt`
12. T2 publie **via le proxy** : `DC mosquitto_pub -h toxiproxy -p 1884 -q 1 -t lab/e -m lent`  
    → la commande met plusieurs secondes à rendre la main (≈ plusieurs allers-retours, chacun +2 s) : QoS 1 attend un accusé.
13. Retirez : `docker compose exec toxiproxy /toxiproxy-cli toxic remove -n latency_downstream mqtt`

## Débrief

- Dans quels cas le « tout va bien » de la fallacy 1 vous a-t-il fait perdre un message sans erreur visible ?
- Votre chat a deux flux : `chat/all` et `chat/dm/<nom>`. Quel QoS pour chacun, et pourquoi ?
- À quoi sert `retain` pour une liste de services ouverts ? Quel danger si un service meurt et que son message retenu reste ?

<details><summary>Solution</summary>

| Cas | Bob reçoit ? |
|---|---|
| A | non (QoS 0 = au plus une fois) |
| B | oui (QoS 1 + session persistante) |
| C | non (pas de session) |
| D | oui, à l'abonnement (retain) |

- Le QoS effectif est le **minimum** entre celui du publieur et celui de l'abonné.
- Chat global : QoS 0 (on tolère une perte). Privé : QoS 1 (mais prévoir les doublons → thématique [Idempotence](../../thematiques/05-idempotence.md)).
- Un retain « ouvert » qui survit à la mort de son auteur ment : d'où le LWT (D04).
</details>
