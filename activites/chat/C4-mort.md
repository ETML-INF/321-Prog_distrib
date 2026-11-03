# C4 — Annoncer sa propre mort (LWT)

| | |
|---|---|
| Thématique | [Microservices](../../thematiques/04-microservices.md) |
| Drill préalable | [D04](../drills/D04-mort-noeud.md) |
| Fallacy | 1 (fiabilité), 5 (topologie) |
| Théorie | [MQTT](../../supports/mqtt.md#last-will-and-testament-lwt) · [Keep-Alive](../../supports/mqtt.md#keep-alive) |

## But
Quand votre chat meurt, les autres le savent sans que vous ayez rien fait.

## Étapes

**1. Déclarez un testament à la connexion.** Topic `chat/presence/<votre prénom>`, QoS 1, **retain**, contenu : une enveloppe `presence` avec `"state":"died"`. Keep-alive : **5 s**.  
→ Attention : le testament est construit **au moment de la connexion**. Son `id` et son `ts` seront ceux de cet instant. Est-ce un problème ? Notez votre réponse.

**2. Mort franche.** Tuez votre chat (`kill -9`) comme dans D04.  
→ Vérifiez : un observateur (`DC mosquitto_sub -t 'chat/presence/#' -v`) voit `died` en 1 à 2 s.

**3. Mort silencieuse.** Connectez votre chat **via le proxy** (`localhost:1884`), puis `docker compose exec toxiproxy /toxiproxy-cli toxic add -t timeout -a timeout=0 mqtt`. Chronométrez.  
→ Vérifiez : `died` apparaît après ≈ 8 à 10 s. Retirez ensuite le toxic.

**4. Affichez-le.** Votre liste des présents (C3) passe l'auteur en `mort` à la réception de `died`.  
→ Vérifiez avec 2 instances : tuez l'une, l'autre affiche « X est mort ».

**5. Renaissance.** Relancez le chat tué.  
→ Vérifiez : à sa connexion il publie `open` (retain), qui **écrase** `died` ; les autres le voient revenir.

## Terminé quand

- [ ] les deux morts sont détectées par un autre chat
- [ ] vous pouvez expliquer pourquoi le délai diffère
- [ ] la renaissance (étape 5) fonctionne

## Si vous utilisez une IA
Prompt : nommer LWT, keep-alive, retain, et donner **les deux scénarios de test** (franche, silencieuse) avec le délai attendu pour chacun.
