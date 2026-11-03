# Résilience (design for failure)

Comment faire en sorte que les pannes soient une donnée contextuelle plutôt qu'une source de désastre ?

**À l'issue de cette thématique, vous serez capable de :**
- choisir entre échec rapide, mode dégradé et nouvelle tentative
- implémenter un backoff exponentiel avec jitter et justifier son plafond
- expliquer le disjoncteur (circuit breaker)
- décrire un arrêt propre (graceful shutdown)

**Fallacies :**

- [1 — Le réseau est fiable](../supports/fallacies.md#fallacy-1)
- [2 — La latence est nulle](../supports/fallacies.md#fallacy-2)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [Design for failure](../supports/design4failure.md)
- [Kahoot P2P / design for failure](https://create.kahoot.it/share/p2p-et-resilience-des-reseaux/20a4a666-6729-4382-8a91-aa8eb8ec1984)

## Drill
- [D07 — Retry : backoff et jitter aux dés](../activites/drills/D07-retry.md)

## Chat
- [C6 — Survivre à la coupure du broker](../activites/chat/C6-reconnexion.md)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Stockage d’état](../activites/fil-rouge/services.md#cat-stockage), [Simulateur d’événements](../activites/fil-rouge/services.md#cat-simulateur)
- **Doit introduire :** une dépendance qui peut tomber (un autre service, le broker)
- **Preuve attendue :** sous panne provoquée avec le proxy, le nœud réessaie avec un backoff, ne sature pas la ressource et reprend seul au retour de la dépendance
- **Décision à documenter :** les timeouts, le nombre d'essais et le plafond du backoff

## Fil rouge

| Fil rouge  | Activité                                                           |
|------------|--------------------------------------------------------------------|
| UMDD       | [Annuaire en cluster](../activites/fil-rouge/umdd/cluster.md)      |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#resilience) |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                 |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |
