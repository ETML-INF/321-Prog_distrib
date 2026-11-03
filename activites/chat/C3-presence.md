# C3 — Présence et messages privés

| | |
|---|---|
| Thématique | [MQTT](../../thematiques/03-mqtt.md) |
| Drill préalable | [D03](../drills/D03-qos-retain.md) |
| Fallacy | 1 (fiabilité), 2 (latence) |
| Théorie | [MQTT](../../supports/mqtt.md) : QoS, session persistante, retain |

## But
Savoir qui est présent dès qu'on arrive, et recevoir ses messages privés même quand on était absent.

## Étapes

**1. Messages privés.** Abonnez-vous à `chat/dm/<votre prénom>` en QoS 1, avec un **identifiant client stable** et une **session persistante**. Envoyez un message à un camarade sur `chat/dm/<son prénom>` en QoS 1, champ `to` renseigné.  
→ Vérifiez : le camarade quitte son chat, vous lui écrivez, il relance son chat : il reçoit le message (D03, cas B).

**2. Annoncez votre présence.** À la connexion, publiez une enveloppe `presence` `{"state":"open"}` sur `chat/presence/<votre prénom>`, QoS 1, **retain activé**. À une sortie propre, publiez `{"state":"closed"}` (retain, QoS 1).  
→ Vérifiez : `DC mosquitto_sub -t 'chat/presence/#' -v -W 3` affiche votre dernier état.

**3. Affichez la liste des présents.** Abonnez-vous à `chat/presence/+`. Tenez à jour une liste « ouverts ».  
→ Vérifiez : injectez à la main un voisin fictif — `DC mosquitto_pub -r -q 1 -t chat/presence/zoe -m '<enveloppe presence open de zoe>'` — votre chat affiche `zoe` **sans attendre** (c'est le retain).  
Puis nettoyez : `DC mosquitto_pub -r -n -t chat/presence/zoe`.

**4. Constatez le défaut.** Tuez votre chat brutalement (Task Manager / `kill -9`). Regardez la liste d'un camarade.  
→ Vous devez observer : vous restez affiché « ouvert ». Le retain a menti. C'est le sujet de [C4](C4-mort.md).

## Terminé quand

- [ ] un DM reçu après absence
- [ ] la liste des présents s'affiche à l'arrivée sans attendre
- [ ] vous avez reproduit le défaut de l'étape 4 et pouvez dire pourquoi il se produit

## Si vous utilisez une IA
Prompt : citez QoS 1, session persistante, retain, le comportement à la sortie brutale (même si on le traite en C4), et le test de l'étape 3.
