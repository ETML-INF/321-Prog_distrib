# D04 — Mort d'un nœud : LWT et keep-alive

| | |
|---|---|
| Thématique | [Microservices](../../thematiques/04-microservices.md) |
| Fallacy | 1 (fiabilité), 5 (topologie) |
| Durée | 25 min |
| Matériel | [labo Docker](../infra/README.md) démarré, 3 terminaux |
| Théorie | [MQTT](../../supports/mqtt.md) : *LWT* et *Keep-Alive* ; [Design for failure](../../supports/design4failure.md) |

## But
Constater qu'il existe deux façons de mourir (franche ou silencieuse) et que la seconde se détecte plus tard.

`DC` = `docker compose exec broker`. Terminaux : **T1** observateur, **T2** Alice, **T3** pour tuer.

## 1. Prédire
Le broker annonce le « testament » (LWT) d'un client quand il constate sa mort. Estimez le délai entre la mort d'Alice et l'annonce, pour :  
(a) Alice est tuée par `kill -9` ; (b) Alice garde sa connexion TCP ouverte mais n'envoie plus rien, avec un keep-alive de 5 s.

## 2. Mort franche (kill)

1. T1 observe la présence : `DC mosquitto_sub -t 'lab/presence/#' -v -W 60`
2. T2 lance Alice avec un testament : 
   ```
   DC mosquitto_sub -i alice -t lab/x -k 5 -W 120 --will-topic lab/presence/alice --will-payload "alice died"
   ```
3. T3 la tue : `DC sh -c 'pkill -9 -f "mosquitto_sub -i alice"'`  
   → T1 affiche **presque aussitôt** `lab/presence/alice alice died` (≈ 1 à 2 s : surtout le coût de la commande Docker). Le système d'exploitation a fermé la connexion TCP, le broker l'a vu.

## 3. Mort silencieuse (câble débranché)

4. T1 observe toujours. T2 lance Dave **via le proxy** :
   ```
   DC mosquitto_sub -h toxiproxy -p 1884 -i dave -t lab/x -k 5 -W 120 --will-topic lab/presence/dave --will-payload "dave died"
   ```
5. T3 « débranche le câble » sans fermer quoi que ce soit : `docker compose exec toxiproxy /toxiproxy-cli toxic add -t timeout -a timeout=0 mqtt`. Chronométrez.  
   → T1 affiche `dave died` après **≈ 8 à 10 s**. Le broker attend 1,5 × le keep-alive (7,5 s) sans nouvelle avant de conclure.
6. Réparez : `docker compose exec toxiproxy /toxiproxy-cli toxic remove -n timeout_downstream mqtt`

## 4. Calculer

7. Un keep-alive de 60 s est un réglage courant. Quel délai de détection d'une mort silencieuse ?  
   → 1,5 × 60 = **90 s**.
8. Votre annuaire doit retirer un nœud mort en moins de 10 s. Quel keep-alive choisir ? Quel est le prix ? (pensez à la fallacy 7)  
   → ≤ 6 s (donc 5 s, minimum de l'outil) ; le prix : un message de contrôle toutes les 5 s par client, sur batterie c'est cher (D10).

## Extension (facultative)
Relancez Alice avec `--will-retain` : un observateur qui arrive *après* la mort voit encore `alice died`. Nettoyez ensuite avec `DC mosquitto_pub -r -n -t lab/presence/alice`.

## Débrief

- Pourquoi un client ne peut-il pas annoncer lui-même sa mort ?
- Pour quel service un détection en 90 s est-elle acceptable ? Pour lequel ne l'est-elle pas ?
- Si le broker ne voit plus Dave mais que Dave, lui, se croit connecté : qui a raison ? (fallacy 1)

<details><summary>Solution</summary>

(a) quasi immédiat, (b) 1,5 × keep-alive = 7,5 s. Le testament est publié par le **broker**, pas par le client : c'est pour cela qu'il fonctionne quand le client ne peut plus rien faire. La détection de mort silencieuse est un compromis détection rapide / trafic de contrôle.
</details>
