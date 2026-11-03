# Cluster, Load Balancing, Failover & Scalability

> **Fallacies :** 5 (topologie constante), 6 (un seul administrateur), 1 (fiabilité).  
> **À retenir :** un cluster = plusieurs nœuds qui se présentent comme un seul service. Le plus simple est de le rendre **sans état** (n'importe quel nœud peut répondre). Dès qu'un nœud détient un état, il faut décider **qui** répond, et le pire cas devient « deux nœuds se croient responsables » (*split brain*).

## 1. Introduction

Une architecture distribuée apporte :

- la capacité à répartir la charge de travail (**load balancing**) ;
- une bonne tolérance aux pannes par la reprise de charge (**failover**) ;
- la souplesse pour augmenter progressivement les ressources (**scalability**).

Ces trois points sont matérialisés au sein d'un **cluster**.

## 2. Le Cluster

> Un **cluster** est un ensemble coordonné de nœuds qui coopèrent pour fournir un service commun.

Exemple : un cluster de centrales de distribution d'énergie. Plusieurs centrales répondent aux demandes ; les maisons ne voient qu'un ensemble logique (un service), pas un serveur unique ; la charge peut être répartie ; la panne d'une centrale ne bloque pas le service.

Un bon cluster réalise **de manière transparente** pour ses utilisateurs :

- la répartition de la charge entre ses nœuds (réduit le temps de traitement apparent) ;
- la redirection d'un travail non terminé vers un autre nœud (la panne devient invisible) ;
- l'ajout d'un nouveau nœud.

## 3. Répartition de charge, aka « Load Balancing »

> Le **load balancing** consiste à diriger une requête entrante vers un nœud fonctionnel et disponible (ou peu chargé).

Deux modes :

1. **Centralisé.** Les requêtes passent par un nœud *Master* qui assigne la tâche à l'un des *Workers*. Algorithmes courants :
   - **Round Robin** : chacun son tour ;
   - **Segmentation** : selon une catégorie du client ;
   - **Least Connections** : au nœud qui a le moins de clients ;
   - **Least Load** : au nœud qui a le moins de charge de travail ;
   - **Random** : au hasard.
2. **Distribué.** Les tâches sont présentées à tous les nœuds, chacun « pioche ». Chaque nœud choisit selon un calcul sur les attributs de la tâche, défini pour qu'aucune tâche ne reste sur le carreau (**Hash-based**).

Avec MQTT 5, les [abonnements partagés](mqtt.md#abonnements-partages-mqtt-5-0) donnent une répartition en round robin sans master.

## 4. Failover

> Le **failover** est le mécanisme qui permet au cluster de continuer à fonctionner quand l'un de ses nœuds tombe.

- Gestion centralisée : le Master ajuste sa liste de Workers.
- Gestion distribuée : chaque nœud restant se reconfigure pour prendre en charge le travail du nœud tombé.

Dans les deux cas, la difficulté est la **détection** de la panne : comment savoir qu'un nœud est mort ?

- **Heartbeat** : chaque nœud émet un signal à intervalle régulier ; son absence signifie la mort.
- **Timeout** : une question est envoyée, la mort est déclarée si la réponse n'arrive pas dans le délai.
- **Détection distribuée** : chaque nœud évalue l'état des autres et vote. Exemple célèbre : les ordinateurs de bord de la navette spatiale.

  ![](./assets/sdC4j.jpg)

### Attention : deux nœuds qui se croient responsables

Quand un nœud est seulement **coupé** du reste (et non mort), il peut continuer à se croire responsable pendant qu'un autre prend sa place : c'est le *split brain*. Pour un service sans état (annuaire), deux réponses identiques sont sans gravité ; pour un service qui détient un état (une banque), c'est un double effet. D'où l'importance de l'[idempotence](idempotence.md). Voir [D08](../activites/drills/D08-cluster.md).

## 5. Scalability

L'augmentation de capacité est l'exact inverse de la panne (sans le problème de détection). On crée le nouveau nœud, puis :

- gestion centralisée : on ajuste la liste de Workers du Master ;
- gestion distribuée : on configure la (nouvelle) règle de répartition sur le nouveau nœud et on reconfigure les autres avec la même règle.
