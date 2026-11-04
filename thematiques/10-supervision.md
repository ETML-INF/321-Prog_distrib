# Supervision (monitoring)

Surveiller un système distribué : savoir qu'il va mal avant que les utilisateurs ne le disent.

**À l'issue de cette thématique, vous serez capable de :**
- distinguer monitoring actif et réactif, SLI / SLO / SLA
- choisir la métrique qui révèle un symptôme donné
- concevoir un heartbeat et en justifier la période
- distinguer une erreur transitoire d'une erreur permanente

**Fallacies :**

- [1 — Le réseau est fiable](../supports/fallacies.md#fallacy-1)
- [7 — Le transport est gratuit](../supports/fallacies.md#fallacy-7)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [Monitoring d'un système informatique complexe](../supports/monitoring.md)
- Slides : [Monitoring](../slides/monitoring/)

## Drill
- [D10 — Monitoring et énergie](../activites/drills/D10-monitoring-energie.md) (parties 1 et 2)

## Chat
- [C8 — Dire comment il va, sobrement](../activites/chat/C8-sante.md)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Supervision](../activites/fil-rouge/services.md#cat-supervision), [Journal](../activites/fil-rouge/services.md#cat-journal)
- **Doit introduire :** un état de santé observable pour chaque nœud (battement ou compteurs)
- **Preuve attendue :** un moniteur externe signale un nœud silencieux après le délai attendu, ni avant ni bien après
- **Décision à documenter :** la période du battement et ce qu'elle coûte (fallacy 7)

## Fil rouge

| Fil rouge  | Activité                                                                    |
|------------|-----------------------------------------------------------------------------|
| UMDD       | [Supervision](../activites/fil-rouge/umdd/supervision.md)                   |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#supervision-energie) |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                          |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |
