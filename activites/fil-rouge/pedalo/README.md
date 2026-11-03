# Fil rouge « Pedaloland » (esquisse)

L'industrie du pédalo est en plein boom (ou pas…). Chacun développe son usine à pédalo ; la ville est faite de services partagés. Descriptif complet : [Pedaloland.md](../../../Pedaloland.md). Projet C# existant : [activites/Pedaloland](../../Pedaloland/README.md).

C'est un **cousin d'UMDD** : même structure de services, autre décor. À réserver à un autre groupe ; cette page ne fait que **mapper** les thématiques.

| Thématique | UMDD | Pedaloland | Notes |
|---|---|---|---|
| Sérialisation | Vitrine (`business`) | `Brand` + `Model` (nom, enseigne, modèle de pédalo) | [schéma business](../../contrats/business.schema.json) à adapter |
| Temps, MQTT | TimeService | horloge commune des usines | idem |
| Microservices | Log + Annuaire | Log + Registre du commerce | idem |
| Idempotence, Transactions | Banque + Coop | Banque + **producteurs de matière première** (fibre de verre, métal, toile) | l'usine achète les composants : transaction à plusieurs fournisseurs |
| Résilience, Cluster | Annuaire en cluster | Registre du commerce en cluster | idem |
| Sécurité | Sécurité | achats signés, commandes confidentielles | idem |
| Supervision, Énergie | Supervision | surveillance des usines et des producteurs | |
| Défi | Défi | à définir | |

## À ajouter pour que ce scénario devienne utilisable

- [ ] une page par thématique illustrée (`serialisation.md`, `time.md`, …) sur le modèle de [UMDD](../umdd/README.md) ;
- [ ] les schémas des messages propres au décor (modèle de pédalo, commande de matière) ;
- [ ] un défi voté spécifique.

Une pièce de pédalo = deux flotteurs (fibre de verre), une coque centrale (fibre de verre), une propulsion (métal), une direction (métal), un parasol (métal et toile). C'est aussi un bon prétexte pour une transaction **multi-fournisseurs** (D06 avec trois fournisseurs).
