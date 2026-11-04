---
theme: default
title: "Sécurité"
info: "Confidentialité, intégrité, authenticité, fraîcheur et leurs primitives"
author: ETML
transition: slide-left
mdc: true
---

# Sécurité

## Quatre propriétés à ne pas confondre

<div class="pt-12">
  <span class="px-2 py-1 rounded bg-blue-500 text-white">
    Fallacy 4 : le réseau est sûr
  </span>
</div>

---

# Plan de la présentation

1. **Les quatre propriétés**
2. **Primitives** - chiffrement, hash, signature
3. **Identité et confiance** - certificats, CA
4. **Fallacies associées** - le piège et comment l'éviter

---
layout: section
---

# Partie 1
## Les quatre propriétés

---

# Quatre questions

| Propriété           | Question                                   | Mécanisme                                    |
|---------------------|--------------------------------------------|----------------------------------------------|
| **Confidentialité** | un tiers qui intercepte comprend-il ?      | chiffrement                                  |
| **Intégrité**       | données identiques à celles envoyées ?     | hash, signature, MAC                         |
| **Authenticité**    | qui est l'auteur ?                         | signature, certificat                        |
| **Fraîcheur**       | message récent, ou rejoué ?                | identifiant + horodatage **dans ce qui est signé** |

<v-click>
<div class="mt-4 p-3 bg-red-700 rounded text-red-100">
Chiffrer ne donne pas les trois autres. La <b>non-répudiation</b> va plus loin : l'émetteur ne peut pas nier (clé privée).
</div>
</v-click>

---
layout: section
---

# Partie 2
## Primitives

---

# Chiffrement

<v-clicks>

- **Chiffrement** : rend la donnée incompréhensible sans la clé. **Encodage** (base64, JSON) : ne protège rien
- **Symétrique** : la **même clé** chiffre et déchiffre. Rapide, mais il faut partager le secret
- **Asymétrique** : paire de clés **publique** et **privée**. Plus lent, aucun secret à partager
- **Clé publique** : diffusée librement, sert à chiffrer pour son propriétaire et vérifier ses signatures
- **Clé privée** : reste secrète, sert à déchiffrer et **signer**

</v-clicks>

---

# Hash, MAC, signature

<v-clicks>

- **Hash** : empreinte de taille fixe, changer une lettre change tout
- **MAC / HMAC** : hash avec **secret partagé**, prouve l'origine dans le groupe et l'intégrité, mais ne distingue pas les membres
- **Signature** : (1) hash de la donnée, (2) chiffrement du hash avec la **clé privée**, jointe au document

</v-clicks>

<v-click>
<div class="mt-4 p-3 bg-blue-700 rounded text-blue-300">
Le destinataire déchiffre avec la clé publique et compare : s'il y parvient, l'émetteur détient la clé privée.
</div>
</v-click>

---
layout: section
---

# Partie 3
## Identité et confiance

---

# Certificats et autorités

<v-clicks>

- **Certificat** : décrit une entité et sa clé publique, **signé par une autorité de certification**
- **CA** : signe avec sa clé privée ; une organisation est CA parce que des millions d'entités ont **sa** clé publique
- **Révocation** : rend un certificat invalide ; listes (CRL) à consulter avant usage

</v-clicks>

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
<b>Exemple :</b> Un message « ouvrir la vanne » intercepté puis rejoué une heure plus tard : sans identifiant ni horodatage signés, il est accepté.
</div>
</v-click>

---
layout: center
class: text-center
---

<v-click every=1>
Confidentialité, intégrité, authenticité, fraîcheur : quatre propriétés, quatre mécanismes.

<div class="pt-12 mb-4">
  <span class="px-4 py-2 rounded bg-blue-500 text-white text-xl">
    Savoir ce qu'on protège, et contre qui
  </span>
</div>

# Questions ?

<div class="mt-8 text-gray-500">

Prochaine étape : confidentialité

</div>
</v-click>
