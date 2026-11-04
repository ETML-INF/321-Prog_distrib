# Cluster et P2P

Plusieurs exemplaires d'un service, ou plus de serveur du tout : qui répond ?

**À l'issue de cette thématique, vous serez capable de :**
- distinguer répartition de charge et élection
- décrire le *split brain* et ses conséquences selon le service
- expliquer ce que le P2P change au rôle du serveur
- choisir ce qui doit être sans état pour faciliter la mise en cluster

**Fallacies :**

- [5 — La topologie ne change pas](../supports/fallacies.md#fallacy-5)
- [6 — Il n’y a qu’un administrateur](../supports/fallacies.md#fallacy-6)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [Cluster, load balancing, failover](../supports/cluster.md)
- [P2P](../supports/P2P.md)
- [Kahoot P2P / design for failure](https://create.kahoot.it/share/p2p-et-resilience-des-reseaux/20a4a666-6729-4382-8a91-aa8eb8ec1984)
- Slides : [Cluster](../slides/cluster/)
- Slides : [P2P](../slides/P2P/)

## Drill
- [D08 — Cluster : qui répond ?](../activites/drills/D08-cluster.md)

## Chat
- (pas d'étape de chat dédiée)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Coordinateur](../activites/fil-rouge/services.md#cat-coordinateur), [Annuaire](../activites/fil-rouge/services.md#cat-annuaire)
- **Doit introduire :** un service répliqué en plusieurs instances, ou un échange direct entre nœuds sans intermédiaire central
- **Preuve attendue :** la perte d'un membre ne coupe pas le service ; les membres restants s'accordent sur l'état
- **Décision à documenter :** la source de vérité, ou comment un coordinateur est élu

## Fil rouge

| Fil rouge  | Activité                                                           |
|------------|--------------------------------------------------------------------|
| UMDD       | [Annuaire en cluster](../activites/fil-rouge/umdd/cluster.md)      |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#resilience) |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                 |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |
