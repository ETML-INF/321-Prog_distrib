# D08 — Cluster : qui répond, et que faire si deux répondent ?

| | |
|---|---|
| Thématique | [Cluster et P2P](../../thematiques/08-cluster-p2p.md) |
| Fallacy | 5 (topologie), 6 (un seul administrateur) |
| Durée | 25 min |
| Matériel | [labo Docker](../infra/README.md) démarré, 3 terminaux ; papier |
| Théorie | [Cluster](../../supports/cluster.md) : *Load Balancing* et *Failover* ; [Design for failure](../../supports/design4failure.md#cluster) |

## But
Deux manières de partager le travail entre instances identiques d'un service (répartir la charge / désigner un seul répondant), et leurs pannes.

`DC` = `docker compose exec broker`. Le broker de Mosquitto supporte les **abonnements partagés** (MQTT 5) : plusieurs abonnés avec la même `$share/<groupe>/<topic>` se partagent les messages.

## Partie 1 : répartir la charge (CLI)

1. T1 : `DC mosquitto_sub -V mqttv5 -i dir1 -t '$share/dir/lab/q' -W 90 -v`
2. T2 : `DC mosquitto_sub -V mqttv5 -i dir2 -t '$share/dir/lab/q' -W 90 -v`
3. T3 : envoyez quatre requêtes, une par une :  
   `DC mosquitto_pub -t lab/q -m req1` (puis `req2`, `req3`, `req4`)  
   → chaque requête arrive **chez un seul** des deux abonnés, en alternance (dir2 reçoit req1, dir1 reçoit req2…).
4. Tuez dir1 (Ctrl+C dans T1) et envoyez `req5`, `req6`.  
   → tout arrive chez dir2 : le **failover** vient gratuitement.

Remarque : `$share` en minuscules, et `-V mqttv5` obligatoires. Une variante sans abonnement partagé (tous reçoivent tout) répond à **chaque requête en double** : vérifiez avec deux abonnés normaux sur le même topic.

## Partie 2 : élire un seul répondant (papier)

Quatre annuaires `d1`…`d4` identiques. Règle proposée : **« le plus petit identifiant vivant répond »**. « Vivant » = a publié `lab/presence/dX` retenu = `open` et son testament (D04) n'est pas passé.

5. Pour chaque scénario, indiquez qui répond et s'il y a un problème :

   | Scénario | Qui répond ? | Problème ? |
   |---|---|---|
   | tout va bien | ? | ? |
   | d1 meurt brutalement (kill) | ? | ? |
   | d1 est coupé du broker (mort silencieuse, keep-alive 5 s) pendant 7,5 s | ? | ? |
   | d1 est coupé du broker, mais **continue de tourner** et se croit vivant | ? | ? |
   | d1 revient après sa panne | ? | ? |

   → d1 · d2, après le testament de d1 (immédiat) · **personne** pendant la détection (≈ 7,5 s) · **d1 et d2** répondent, c'est le *split brain* · d1 reprend (et d2 doit s'effacer).

6. Qu'est-ce qui est le plus grave : « personne ne répond pendant 8 s », ou « deux répondent » ? Répondez pour (a) un annuaire qui renvoie la liste des services ouverts ; (b) une banque qui exécute des transferts.  
   → (a) deux réponses identiques : sans conséquence, le client prend la première. (b) deux exécutions : **double paiement** sauf si la banque est idempotente (D05).

## Débrief

- Que gagne-t-on à *réduire* le besoin d'élection en rendant le service sans état (stateless) ?
- Quelle propriété de l'enveloppe rend les doubles réponses inoffensives pour l'annuaire ?

<details><summary>Solution</summary>

Un service sans état n'a pas besoin d'un seul répondant : on prend le premier qui répond (ou l'abonnement partagé) et on ignore les doublons grâce à l'`id`. L'élection n'est nécessaire que si l'instance **détient un état** qu'on ne peut pas dupliquer (la banque).
</details>
