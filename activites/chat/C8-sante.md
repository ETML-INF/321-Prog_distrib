# C8 — Dire comment il va, sobrement

| | |
|---|---|
| Thématique | [Supervision](../../thematiques/10-supervision.md) |
| Drill préalable | [D10](../drills/D10-monitoring-energie.md) |
| Fallacy | 7 (le transport est gratuit) |
| Théorie | [Monitoring](../../supports/monitoring.md) · [Puissance et énergie](../../supports/Puissance%20et%20énergie.md) |

## But
Votre chat publie régulièrement de quoi diagnostiquer sa santé, sans gaspiller le réseau.

## Étapes

**1. Comptez.** Ajoutez des compteurs : messages envoyés, reçus, invalides ignorés, doublons ignorés, reconnexions.

**2. Publiez un battement.** Toutes les `N` secondes, publiez une enveloppe `log` de niveau `info` sur `chat/log/<votre prénom>` dont le texte suit la [convention de battement](../contrats/README.md#battement-heartbeat) (`hb` suivi de paires `clé=valeur`), par exemple `hb sent=12 recv=40 bad=2 dup=1 reco=0`.  
→ Vérifiez : `DC mosquitto_sub -t 'chat/log/#' -v` affiche vos battements.

**3. Choisissez `N`.** Trop court : du trafic et de l'énergie perdus (D10). Trop long : une panne reste invisible. Calculez la détection d'une mort silencieuse (silencieux après plus de `3 × N` secondes sans battement) et le nombre de messages par heure pour `N` = 5, 30, 300.  
→ Notez votre choix et sa justification dans `NOTES.md`.

**4. Détectez de l'extérieur.** Écrivez un petit programme (ou un script) qui écoute `chat/log/#` et signale « X silencieux depuis plus de 3 N secondes ».  
→ Vérifiez : tuez un chat, le moniteur le signale après le délai attendu, pas avant.

**5. Logguez les transitions d'erreur**, pas chaque occurrence (apparition et disparition d'une panne : voir *erreur transitoire*, [Monitoring](../../supports/monitoring.md)).

## Terminé quand

- [ ] battement visible, avec compteurs
- [ ] `N` choisi et justifié par un calcul
- [ ] un chat tué est signalé par votre moniteur
- [ ] vous avez réuni deux battements successifs pour calculer un **taux** (messages par seconde)

## Si vous utilisez une IA
Prompt : nommer heartbeat, la période `N` **et son calcul**, le seuil de détection, ce qui est publié (compteurs précis) et le test (tuer un chat).
