# Les 8 erreurs des systèmes distribués (fallacies)

Formulées chez Sun Microsystems (L. Peter Deutsch, James Gosling et d'autres) : ce sont les croyances que l'on a *par défaut* sur un réseau, et qui sont fausses. Chaque erreur finit en panne en production.

| # | On croit que… | En réalité… | Parade | Théorie |
|---|---|---|---|---|
| <span id="fallacy-1"></span>1 | Le réseau est fiable | les messages se perdent, se dupliquent, les nœuds meurent | accusés de réception, retry, idempotence, LWT | [Design for failure](design4failure.md) · [Idempotence](idempotence.md) · [MQTT](mqtt.md#last-will-and-testament-lwt) |
| <span id="fallacy-2"></span>2 | La latence est nulle | elle varie, elle explose | timeout, asynchrone | [Design for failure](design4failure.md#timeout) |
| <span id="fallacy-3"></span>3 | La bande passante est infinie | elle est limitée et partagée | formats compacts, ne pas tout envoyer | [Sérialisation](Sérialisation.md) |
| <span id="fallacy-4"></span>4 | Le réseau est sûr | quelqu'un écoute, modifie, rejoue | chiffrement, signature | [Sécurité](securite.md) · [Confidentialité](confidentialite.md) |
| <span id="fallacy-5"></span>5 | La topologie ne change pas | des nœuds arrivent, partent, bougent | découverte (annuaire), pas d'adresses codées en dur | [Microservices](microservice.md) · [Cluster](cluster.md) |
| <span id="fallacy-6"></span>6 | Il n'y a qu'un administrateur | plusieurs équipes, plusieurs règles | contrats explicites, versionnés | [Réseau de confiance](reseau-confiance.md) |
| <span id="fallacy-7"></span>7 | Le transport est gratuit | il coûte du temps, de l'énergie, de l'argent | mesurer, grouper, espacer | [Puissance et énergie](Puissance%20et%20énergie.md) |
| <span id="fallacy-8"></span>8 | Le réseau est homogène | langages, versions, systèmes différents | formats ouverts, contrat | [Sérialisation](Sérialisation.md) |

## Un neuvième fil rouge : l'heure {#heure}
« À tout endroit du réseau, chacun connaît l'heure avec précision. » Faux aussi : voir [NTP](ntp.md) et l'[alternative logique](ntp.md#alternative-logique) (Lamport).

## Comment s'en servir dans ce module

- **Pour lire un énoncé** : « de quelle croyance cette fonctionnalité dépend-elle ? »
- **Pour écrire un prompt** : nommer la fallacy menaçante est un critère d'évaluation, voir la [grille](../evaluation/grille.md).
- **Pour débugger** : avant de chercher le bug, cherchez la fallacy.
