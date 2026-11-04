---
theme: default
title: "Réseau de confiance"
info: "Identité et validité : qui est-ce, et a-t-il le droit de le faire ?"
author: ETML
transition: slide-left
mdc: true
---

# Réseau de confiance

## Identité et validité

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacies 4 et 6
  </span>
</div>

---
layout: image
image: ./assets/confiance.jpg
---

---

# Plan de la présentation

1. **Deux aspects** - identité, validité
2. **PKI** - signer les messages MQTT
3. **Défis** - certificats, latence, confiance initiale
4. **Dans ce module**
5. **Fallacies associées** - le piège et comment l'éviter

---
layout: section
---

# Partie 1
## Deux aspects fondamentaux

---

# Identité

<v-clicks>

- Le récepteur est certain de l'identité de l'émetteur
- L'émetteur ne peut pas nier : **non-répudiation**, architecture **PKI**
- **Certificats** émis par une autorité centrale ou décentralisée
- **Propagation** de la confiance : directe (A connaît B) ou transitive (A fait confiance à B, qui fait confiance à C)
- **Validation mutuelle** avant de partager

</v-clicks>

---

# Validité

<v-clicks>

- Une identité vérifiée ne dit pas que les actions **respectent les règles**
- Contrôle, traçabilité, éventuellement consensus distribué
- **Logging** centralisé dans un microservice, messages de log **signés** avec la clé privée de l'émetteur

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
Une signature répond à « qui est-ce ? », pas à « a-t-il le droit ? ».
</div>
</v-click>

---
layout: section
---

# Partie 2
## PKI et MQTT

---

# Signer les messages

<v-clicks>

1. **Confiance** : chaque maison obtient un certificat X.509 auprès d'une CA
2. **Signature** : message publié accompagné de la signature de son contenu (clé privée)
3. **Vérification** : les abonnés utilisent la clé publique du certificat : origine et données non modifiées
4. **Fichiers volumineux** : hacher (SHA-256), signer le hash

</v-clicks>

---

# Diagramme de séquence

```mermaid {scale: 0.5}
sequenceDiagram
    participant A as Maison A
    participant Broker as Broker MQTT
    participant B as Maison B
    participant CA as Autorité de certification
    A->>CA: Demande un certificat
    CA-->>A: Certificat avec clé publique
    A->>A: Signe le message avec sa clé privée
    A->>Broker: Publie le message signé
    Broker-->>B: Transmet
    B->>CA: Demande le certificat de A
    CA->>B: Certificat de A
    B->>B: Vérifie le certificat avec la clé publique de la CA
    B->>B: Vérifie la signature avec la clé publique de A
```

---
layout: section
---

# Partie 3
## Défis

---

# Défis et pistes

| Défi                       | Problème                                  | Piste                                       |
|----------------------------|-------------------------------------------|---------------------------------------------|
| **Gestion des certificats**| distribution et **révocation**            | vérifier par un protocole comme **OCSP**    |
| **Latence et surcoût**     | la cryptographie ajoute de la latence     | ECDSA plutôt que RSA, puces TPM             |
| **Confiance initiale**     | pas de point central fort                 | autorité initiale décentralisée, blockchain |

---

# Dans ce module

<v-clicks>

- Pas d'autorité de certification officielle
- La classe **décide ensemble** qui atteste de l'identité de qui (annuaire ? enseignant ? accord de la classe ?)
- Et ce qui se passe quand une clé fuit

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-yellow-600 rounded text-yellow-200">
C'est exactement le problème de la confiance initiale.
</div>
</v-click>

---
layout: section
---

# Fallacies associées
## Le piège et comment l'éviter

---

# Fallacy 4 : le réseau est sûr

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Quelqu'un écoute, modifie ou rejoue les messages ; le broker lui-même est un intermédiaire.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Chiffrer, signer, inclure identifiant et horodatage dans ce qui est signé, valider les entrées, authentifier les clients.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Un pair malveillant publie au nom d'un autre. Signature avec clé privée et certificat vérifié : l'imposteur est rejeté.
</div>
</v-click>

---

# Fallacy 6 : il n'y a qu'un administrateur

<v-click>
<div class="mt-1 p-3 bg-red-700 rounded text-red-100">
<b>Ce qui arrive :</b> Plusieurs équipes déploient à leur rythme avec leurs règles : une modification locale casse les autres.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-green-700 rounded text-green-200">
<b>Mieux faire :</b> Contrats explicites et versionnés, compatibilité ascendante, journaux signés, décisions de confiance documentées.
</div>
</v-click>

<v-click>
<div class="mt-2 p-3 bg-blue-800 rounded text-blue-100">
<b>Exemple :</b> Qui atteste de l'identité de qui ? La classe décide ensemble et documente ce qui se passe quand une clé fuit.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Deux questions distinctes : l'identité et la validité.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Signer prouve l'identité, pas le droit d'agir
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : monitoring

</div>
</v-click>
