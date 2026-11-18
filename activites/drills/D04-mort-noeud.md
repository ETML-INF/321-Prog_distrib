# D04 — Mort d'un nœud : LWT et keep-alive

| | |
|---|---|
| Thématique | [Microservices](../../thematiques/04-microservices.md) |
| Fallacy | 1 (fiabilité), 5 (topologie) |
| Durée | 35 min |
| Matériel | [labo Docker](../infra/README.md) démarré, 3 terminaux |
| Théorie | [MQTT](../../supports/mqtt.md) : *LWT* et *Keep-Alive* ; [Design for failure](../../supports/design4failure.md) ; Validation : [Valider à la frontière](../../supports/microservice.md#valider-a-la-frontiere) |

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

## 5. Valider le testament

**Prédire d'abord :** le testament d'Alice est-il un message valide du [contrat](../contrats/README.md) ?

Dans les étapes 2 à 6, le testament était du texte brut. Dans le contrat, c'est une enveloppe `presence`. Outil : `ajv-cli` (Node requis), depuis le dossier `activites/contrats`. Le message reçu par T1 :

```json
{"v":1,"id":"0b5f5c0e-3a52-4c58-9b0e-5a3f2b1a9c11","from":"alice","to":"*","ts":"2026-11-02T08:15:30Z","type":"presence","payload":{"state":"died"}}
```

**Outil graphique :** [JSONLint — JSON Schema](https://jsonlint.com/json-schema){target="_blank"} (schéma à gauche, données à droite). Il convient pour le `payload` contre `presence.schema.json` (étapes 9 et 10), mais il **refuse** `envelope.schema.json` (*Schema error: unknown format "uuid"*) : il ne gère pas `format`. Pour l'enveloppe, utilisez `ajv-cli`.

Enregistrez-le dans `testament.json`, et son `payload` seul (`{"state":"died"}`) dans `payload.json`. Commande de base (`-c ajv-formats` active l'assertion de format) :

```
npx ajv-cli validate --spec=draft2020 -c ajv-formats -s envelope.schema.json -d testament.json
```

9. Validez `testament.json` contre `envelope.schema.json`, puis `payload.json` contre `presence.schema.json` → **valide** deux fois.
10. Cassez : `"state":"dead"` dans `payload.json` → **refusé** (hors de l'énumération).
11. Cassez : retirez `id` de `testament.json` → **refusé** (champ obligatoire absent).
12. Mettez `"id":"nope"`. Validez **sans** l'assertion de format : `npx ajv-cli validate --spec=draft2020 --strict=false -s envelope.schema.json -d testament.json` → `valid` (seul un avertissement *unknown format "uuid" ignored* apparaît). Refaites avec `-c ajv-formats` → **refusé**. Un `format` n'est vérifié que si on le demande.

## Extension (facultative)
Relancez Alice avec `--will-retain` : un observateur qui arrive *après* la mort voit encore `alice died`. Nettoyez ensuite avec `DC mosquitto_pub -r -n -t lab/presence/alice`.

## Débrief

- Pourquoi un client ne peut-il pas annoncer lui-même sa mort ?
- Pour quel service un détection en 90 s est-elle acceptable ? Pour lequel ne l'est-elle pas ?
- Si le broker ne voit plus Dave mais que Dave, lui, se croit connecté : qui a raison ? (fallacy 1)
- Qui valide, le broker ou le récepteur ? Pourquoi le broker ne peut-il pas valider le testament ?
- Le `ts` du testament est celui de la connexion : valide, mais que vaut-il ? (voir [C4](../chat/C4-mort.md), étape 1)

<details><summary>Solution</summary>

(a) quasi immédiat, (b) 1,5 × keep-alive = 7,5 s. Le testament est publié par le **broker**, pas par le client : c'est pour cela qu'il fonctionne quand le client ne peut plus rien faire. La détection de mort silencieuse est un compromis détection rapide / trafic de contrôle.

Validation : le broker transporte des octets, il ne connaît pas le contrat ; la garde est chez le **récepteur**. Le testament est valide mais son `ts` (et son `id`) datent de la connexion : il ne dit rien de l'heure de la mort. Enfin, `format` n'est assertif que si la bibliothèque est configurée pour (étape 12).
</details>
