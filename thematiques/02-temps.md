# Temps

![ntp.webp](assets/ntp.webp)

Tic, tac… Qui a raison quand deux horloges se contredisent ?

**À l'issue de cette thématique, vous serez capable de :**
- expliquer ce que NTP corrige et ce qu'il ne corrige pas
- dire quand l'ordre suffit et quand l'heure réelle est nécessaire (horloges logiques)
- calculer un décalage et un délai à partir de quatre horodatages
- mesurer l'effet d'une latence sur un décalage

**Fallacies :**

- [Chacun connaît l’heure avec précision](../supports/fallacies.md#heure) (le « neuvième » fil rouge)
- [2 — La latence est nulle](../supports/fallacies.md#fallacy-2)

Voir [les 8 fallacies](../supports/fallacies.md).

## Théorie
- [NTP](../supports/ntp.md)
- [Kahoot NTP](https://create.kahoot.it/share/ntp/b18a5c33-0312-451b-8fb7-52b6a6fca6bc)

## Drill
- [D02 — Temps : trier des messages quand les horloges mentent](../activites/drills/D02-temps.md)

## Chat
- [C2 — Ordre des messages](../activites/chat/C2-ordre.md)

## Exigences du fil rouge

Quel que soit le scénario choisi, il doit, pour cette thématique :

- **Services attendus :** [Source de temps](../activites/fil-rouge/services.md#cat-source-de-temps), [Simulateur d’événements](../activites/fil-rouge/services.md#cat-simulateur)
- **Doit introduire :** une source d'heure commune à tous les nœuds, et au moins une donnée qui dépend de l'heure
- **Preuve attendue :** tous les nœuds affichent la même heure à moins d'une seconde ; avec un décalage d'horloge injecté (`--clock-skew 30`), l'écart est visible
- **Décision à documenter :** la fréquence de diffusion et ce que fait un nœud qui ne reçoit plus l'heure (mode dégradé)

## Fil rouge

| Fil rouge  | Activité                                                                |
|------------|-------------------------------------------------------------------------|
| UMDD       | [TimeService](../activites/fil-rouge/umdd/time.md)                      |
| Powercher  | [Powercher](../activites/fil-rouge/powercher/README.md#temps-protocole) |
| Pedaloland | [Mapping](../activites/fil-rouge/pedalo/README.md)                      |
| Libre      | [Fil rouge libre](../activites/fil-rouge/libre/README.md) (à construire avec le [gabarit](../activites/fil-rouge/modele/README.md)) |

Activité alternative : [Manipulations temporelles](../activites/ntp1/README.md)
