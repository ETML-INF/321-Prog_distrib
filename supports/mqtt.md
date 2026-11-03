# MQTT

> **Fallacies :** 1 (le réseau est fiable), 2 (latence nulle).  
> **À retenir :** MQTT est un bus *publish-subscribe* léger. Il ne vous garantit rien de plus que ce que vous demandez : **QoS** (livraison), **session** (absence), **retain** (arrivée tardive), **LWT** (mort). Chaque garantie a un prix et une limite.

Le protocole MQTT (Message Queuing Telemetry Transport) est conçu pour des échanges rapides à faible bande passante, typiquement l'Internet des objets (IoT) : peu de ressources, connexions instables. Développé dans les années 1990, il repose sur une architecture *publish-subscribe* : les clients ne se parlent pas directement, ils passent par un serveur central, le **broker**.

![](./assets/mqtt.jpg)

Les clients se connectent au broker en TCP/IP, **publient** des messages sur des **topics**, et d'autres clients s'**abonnent** à ces topics. Un capteur publie sur `moteur/température`, tous les abonnés reçoivent.

Les topics sont hiérarchiques, séparés par `/` : `maison/salon/temperature` = bâtiment / pièce / type d'information.

## Utilisation des Wildcards

Pour s'abonner à plusieurs topics à la fois :

- `+` remplace **un** niveau : `maison/+/temperature` = la température de toutes les pièces ;
- `#` remplace **plusieurs** niveaux (en fin de filtre) : `maison/#` = tout ce qui concerne la maison.

## Qualité de Service (QoS)

MQTT offre plusieurs mécanismes pour assurer un service malgré des événements imprévus.

### QoS

| QoS | Garantie | Coût | Conséquence |
|---|---|---|---|
| 0 | au plus une fois, sans accusé | le plus léger | un message peut se perdre |
| 1 | **au moins une fois** | un accusé (PUBACK) | un message peut arriver **en double** → idempotence ([Idempotence](idempotence.md)) |
| 2 | exactement une fois | 4 messages d'échange | le plus lent |

Le QoS effectif d'une livraison est le **minimum** entre celui du publieur et celui de l'abonné.

### Last Will and Testament (LWT)

Mécanisme qui informe les autres quand un client disparaît sans prévenir. À sa connexion, le client dépose chez le broker un message « testament » (topic, contenu, QoS, retain).

- Déconnexion **propre** (le client envoie `DISCONNECT`) : le testament **n'est pas** publié.
- Déconnexion **anormale** (panne de réseau, coupure de courant, processus tué) : le broker publie lui-même le testament.

C'est le **broker** qui publie, pas le client : voilà pourquoi cela fonctionne quand le client ne peut plus rien faire. Le contenu du testament est figé à la connexion (son horodatage aussi). Voir [D04](../activites/drills/D04-mort-noeud.md).

### Keep-Alive

Intervalle fixé par le client à la connexion : s'il n'envoie rien pendant ce temps, il envoie un `PINGREQ` (le broker répond `PINGRESP`). Si le broker ne reçoit **rien pendant 1,5 × Keep-Alive**, il considère le client mort, ferme la connexion et publie son LWT.

Compromis : un keep-alive court détecte vite une **mort silencieuse** (câble débranché, pas de fermeture TCP) mais génère du trafic et consomme de l'énergie.

### Session Persistante

Un client peut demander une session persistante (identifiant client stable, session non « propre »). Le broker conserve alors ses **abonnements** et les **messages QoS 1 et 2** non délivrés pendant son absence, et les lui remet à la reconnexion. Utile pour les appareils à connexion intermittente (capteurs sur batterie). Les messages QoS 0 ne sont pas conservés.

### Retained Messages (Messages conservés)

Le broker garde le **dernier** message retenu d'un topic et le remet immédiatement à tout nouvel abonné. Utile pour un état courant (« le service est ouvert », une alerte). Danger : un état retenu peut **mentir** (le client est mort, son dernier message dit encore « ouvert ») : on le corrige avec un LWT retenu. Pour effacer un message retenu : publier un contenu vide avec retain.

### Bufferisation

Quand un client perd sa connexion, il peut continuer à fonctionner et stocker localement les messages à émettre dans une file, envoyés dans l'ordre à la reconnexion. Prévoir une **taille maximale** : une file infinie sature la mémoire.

### NoLocal

Par défaut, un client abonné à un topic sur lequel il publie reçoit **ses propres messages**. L'option de souscription *NoLocal* (MQTT 5.0 uniquement) demande au broker de ne pas les lui renvoyer. Avec MQTT 3.1.1, on filtre soi-même (par l'émetteur déclaré dans le message).

Cas typique : deux brokers reliés en pont, pour éviter une boucle infinie de messages.

### Abonnements partagés (MQTT 5.0)

Avec un filtre `$share/<groupe>/<topic>`, les abonnés d'un même groupe **se partagent** les messages : chaque message n'arrive que chez l'un d'eux, à tour de rôle. C'est de la répartition de charge et du *failover* intégrés (voir [Cluster](cluster.md)).
