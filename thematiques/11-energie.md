# Énergie (puissance vs énergie)

Un message coûte de l'énergie : à partir de quelle fréquence devient-il un problème ?

**À l'issue de cette thématique, vous serez capable de :**
- distinguer puissance et énergie (énergie = puissance × temps)
- calculer l'énergie dépensée par une fréquence d'envoi
- calculer l'autonomie d'un appareil sur batterie
- arbitrer précision de mesure et durée de vie

**Fallacies :**

- [7 — Le transport est gratuit](../supports/fallacies.md#fallacy-7)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [Puissance et énergie](../supports/Puissance%20et%20énergie.md)
- [Kahoot récap](https://create.kahoot.it/details/869bfcbc-6e29-414a-82eb-8764bef04efd)
- Slides : [Puissance et énergie](../slides/puissance-energie/){target="_self"}

## Drill
- [D10 — Monitoring et énergie](../activites/drills/D10-monitoring-energie.md) (partie 3)

## Chat
- [C8 — Dire comment il va, sobrement](../activites/chat/C8-sante.md) (choix de la période du battement)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Supervision](../activites/fil-rouge/services.md#cat-supervision), [Simulateur d’événements](../activites/fil-rouge/services.md#cat-simulateur)
- **Doit introduire :** une grandeur qui s'accumule ou se consomme dans le temps, avec des flux entrants et sortants (énergie : puissance × durée ; mais aussi stock, crédit ou bande passante)
- **Preuve attendue :** le bilan reste cohérent quand des messages se perdent ou arrivent en retard
- **Décision à documenter :** ce que coûte la supervision elle-même (fréquence des mesures, taille des messages)

## Fil rouge

| Fil rouge  | Activité                                                                    |
|------------|-----------------------------------------------------------------------------|
| UMDD       | [Supervision](../activites/fil-rouge/umdd/supervision.md)                   |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#supervision-energie) |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                          |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |

Activité alternative : bilan énergétique d'une maison, voir [Powercher](../activites/fil-rouge/powercher/README.md#supervision-energie)
