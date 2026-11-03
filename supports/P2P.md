# Architecture P2P

> **Fallacies :** 5 (topologie), 6 (plusieurs administrateurs).  
> **À retenir :** en P2P, chaque nœud est à la fois client et serveur. On supprime le serveur central (et son point de défaillance), mais chaque nœud doit tout savoir faire, et trouver ses pairs devient le problème.

![p2p.gif](assets/p2p.gif)

## Rappels

- **Client-serveur** : un client (ordinateur ou application) demande des services ou des données à un serveur centralisé, qui répond. Exemple : un navigateur demande une page à un serveur web.
- **Microservices** : le système est décomposé en services indépendants et autonomes, déployables sur des nœuds distants ([Microservices](microservice.md)).

## Architecture Peer-to-Peer (P2P)

Le P2P combine des aspects des deux :

- chaque nœud (*peer*) agit à la fois comme **client et serveur** ;
- les peers sont déployés sur des machines distinctes, reliées par un réseau commun ;
- on les nomme « peers » parce qu'ils ont tous les **mêmes fonctionnalités**.

C'est une architecture distribuée : elle présente les mêmes avantages et inconvénients que les microservices. Elle limite en plus l'**évolutivité fonctionnelle** : tous les nœuds étant similaires, tous doivent implémenter toutes les fonctionnalités. Exemple : partage de fichiers avec BitTorrent.

## Et avec un broker ?

Un broker MQTT est un point central : on n'est pas en P2P pur. On peut toutefois s'en approcher : les pairs s'**annoncent** (présence), se **découvrent** (annuaire ou retain) et s'adressent directement par des topics dédiés (un topic par destinataire, comme un identifiant de destinataire dans le message ou `chat/dm/<nom>` dans le chat). Le broker n'est plus qu'un tuyau, mais reste un point de défaillance : voir [Cluster](cluster.md).

## Conclusion

Le choix entre P2P et client-serveur dépend des besoins (scalabilité, coût, sécurité). Les deux architectures ont leurs usages et se complètent souvent dans les environnements modernes.
