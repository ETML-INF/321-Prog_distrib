# D10 — Monitoring et énergie

| | |
|---|---|
| Thématique | [Supervision](../../thematiques/10-supervision.md) |
| Fallacy | 7 (le transport est gratuit) |
| Durée | 25 min |
| Matériel | [labo Docker](../infra/README.md) démarré, 2 terminaux ; calculatrice |
| Théorie | [Monitoring](../../supports/monitoring.md) · [Puissance et énergie](../../supports/Puissance%20et%20énergie.md) |

## But
(1) Lire ce que le broker dit de lui-même. (2) Associer un symptôme à la métrique qui le révèle. (3) Chiffrer le coût énergétique de la fréquence d'envoi.

`DC` = `docker compose exec broker`.

## 1. Le broker se surveille (monitoring réactif)

Mosquitto publie ses métriques sur le topic `$SYS/#`.

1. T1 : `DC mosquitto_sub -t '$SYS/broker/clients/connected' -t '$SYS/broker/messages/received' -v -W 15`  
   → deux lignes immédiatement (valeurs retenues), par exemple `$SYS/broker/clients/connected 2`. Les chiffres varient.
2. T2 : ouvrez 3 abonnés (`DC mosquitto_sub -t lab/z -W 60`) dans trois terminaux, relancez la commande 1.  
   → `clients/connected` a augmenté d'autant. Le **nombre de clients** est un indicateur de santé.
3. Tuez-en un brutalement ; combien de temps avant que le compteur ne baisse ? (comparez avec D04)  
   → instantané pour une fermeture franche, jusqu'à 1,5 × keep-alive pour une mort silencieuse.

## 2. Quelle métrique pour quel symptôme ? (papier)

| Symptôme observé | Métrique qui le révèle |
|---|---|
| Les messages privés arrivent avec 30 s de retard | ? |
| Les virements se font deux fois | ? |
| Plus rien de l'annuaire, mais le broker est « up » | ? |
| Le broker devient lent après le lancement du Black Friday | ? |

Choix : latence p95 · messages reçus par seconde · écart entre deux battements (heartbeat) · nombre de doublons (même `id`) détectés · nombre de clients connectés.

→ latence p95 (ou écart `ts` d'émission / de réception) · doublons détectés · écart de heartbeat · messages/s (saturation).

## 3. Combien coûte un message ? (calcul)

Un capteur sur batterie (valeurs **inventées pour l'exercice**) :

- batterie : 2000 mAh à 3,7 V → énergie = 2 Ah × 3,7 V = **7,4 Wh**
- veille : 0,001 W
- chaque publication coûte 0,0005 Wh

4. Durée de vie s'il publie **toutes les secondes** ?  
   → 3600 messages/h × 0,0005 = 1,8 Wh/h, plus 0,001 Wh/h de veille → 1,801 W ⇒ 7,4 / 1,801 ≈ **4,1 h**.
5. **Toutes les minutes** ?  
   → 60 × 0,0005 = 0,03 ; + 0,001 → 0,031 W ⇒ 7,4 / 0,031 ≈ 239 h ≈ **10 jours**.
6. **Toutes les 10 minutes** ?  
   → 6 × 0,0005 = 0,003 ; + 0,001 → 0,004 W ⇒ 1850 h ≈ **77 jours**.
7. Le keep-alive de D04 (5 s) coûte aussi un message de contrôle : combien par heure ? Comparez avec les cas 4 et 6 (on suppose un coût égal à celui d'une publication, en réalité un peu moindre). Qu'en concluez-vous pour un appareil sur batterie ?  
   → 3600 / 5 = **720 messages de contrôle par heure** : +20 % à côté des 3600 du cas 4, mais **120 fois** les 6 messages utiles du cas 6. Un keep-alive court ruine un capteur qui publie rarement.

## Débrief

- Pour détecter un capteur mort, vaut-il mieux l'interroger (polling) ou qu'il se signale (heartbeat) ? Que coûte chaque option ? (voir *Monitoring actif / réactif*)
- Que faire quand la précision de la mesure exige de la fréquence et que la batterie l'interdit ? (envoyer uniquement les changements, regrouper)

<details><summary>Solution</summary>

Le monitoring réactif coûte moins de trafic, mais l'absence de signal est ambiguë (mort ? silence normal ?) : il faut un heartbeat dont l'écart est lui-même la métrique. Le polling donne un état net à un instant mais coûte un aller-retour par cible à chaque tour.
</details>
