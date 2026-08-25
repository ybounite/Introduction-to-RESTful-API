# Salting & Slow Hashing

## Defending Against Real Attacks

### Introduction

In the previous lesson, you learned why passwords must be hashed and how systems compare hashes without knowing the password.

But here is the hard truth:

> Hashing alone is NOT enough in real systems.

Attackers do not try to reverse hashes. They guess passwords, hash those guesses, and compare the results.

This lesson explains how real systems defend against that.

### Outcome

You will understand:

- Why simple hashing fails
- What salting really does
- Why slow hashing protects users
- How modern systems store passwords safely

---

### The Real Attack: Offline Password Cracking

Let’s think like an attacker.

If attackers steal your database, they may see something like:

```text
user@email.com -> a94a8fe5ccb19ba...
```

They can then:

- Guess passwords
- Hash each guess
- Compare the results
- Repeat this millions of times per second

> This is called an offline attack.

Important points:

- No rate limits
- No login endpoint
- No alerts
- The attacker is working on stolen data, not a live system

---

### Why Fast Hashing Is Dangerous

Common algorithms like:

- SHA-256
- MD5

are:

- Very fast
- Designed for integrity, not passwords

> Fast hashing means fast cracking.

---

### What Is Salting?

Salting means adding random data to a password before hashing it.

![Salt illustration](https://uploads.teachablecdn.com/attachments/c80a1bb21b9c4cdb83a7afbb3761a116.png)

A salt is a unique, random value added to each password before hashing.

---

### Why Salting Exists

Without salting:

- Same passwords produce the same hashes
- Attackers can use rainbow tables
- Precomputed attacks become very effective

A rainbow table is a precomputed list of hashes created from common passwords. Attackers use it to crack hashed passwords more quickly.

With salting:

- Same passwords produce different hashes
- Precomputed attacks fail
- Each user gets a unique password recipe

---

### Example (Simple)

Without salt:

```text
Hash("Password123") -> ABC123
Hash("Password123") -> ABC123
```

With salt:

```text
Hash("Password123" + "X9$A") -> H1
Hash("Password123" + "Q2!M") -> H2
```

> Same password. Completely different hashes.

---

### Important Clarification

Salts are NOT secret.

They are:

- Stored in the database
- Unique per user
- Used to defeat precomputed attacks

Security comes from:

- randomness
- uniqueness

### Mental Model

**Salt = Unique Recipe per User**

Even if the attacker knows the salt:

- They must cook each dish separately
- They cannot reuse results
- It becomes slow and expensive

> Even if the attacker knows the salt, they still must hash each guess for each user every time.

That is where slow hashing finishes the job.

---

### What Is Slow Hashing?

Slow hashing intentionally makes hashing expensive and slow.

Slow hashing algorithms are designed to:

- Take measurable time
- Consume CPU and memory
- Be expensive to brute-force

---

### Why Slow Hashing Matters

Attackers rely on speed.

If:

- One hash = 1 ms
- 1,000 guesses = 1 second

Then with slow hashing:

- One hash = 500 ms
- 1,000 guesses = 500 seconds

> Slow hashing turns brute-force into a nightmare.

---

### Password Hashing Algorithms (Correct Ones)

These are the correct choices for passwords:

- bcrypt
- Argon2 (modern and recommended)
- PBKDF2

These provide:

- Built-in salting
- Adjustable slowness
- Better defense against GPU attacks

---

### What Not to Use for Passwords?

Avoid:

- MD5
- SHA-1
- SHA-256 by itself
- Base64

> If it is fast, it is dangerous for passwords.

---

### Mental Model (Very Important)

**Password Defense = Lock + Noise + Time**

- Hashing -> Lock
- Salting -> Unique lock per user
- Slow hashing -> Time-wasting defense

> Attackers do not break in; they give up.

---

### Complete Secure Password Flow

1. User sends password
2. API generates a random salt
3. Password + salt -> slow hash
4. Hash + salt stored in the database
5. Original password is discarded

> The password never exists after hashing.

---

### Common Questions

#### If salt is stored, doesn’t that weaken security?

No.

Salts:

- Prevent rainbow tables
- Prevent identical hashes
- Do not need secrecy

#### Why not encrypt passwords instead?

Encryption is reversible.

If the encryption key leaks:

- All passwords may be exposed

> Hashing + salt + slowness avoids this problem entirely.

---

### Reality Check

- Attackers steal databases, not passwords
- Hashing alone is insufficient
- Speed helps attackers, not defenders
- Time is your strongest weapon

---

### Interconnection

- Hashing -> one-way protection
- Salting -> uniqueness
- Slow hashing -> brute-force resistance

Together, they create real password security.

---

### Summary of Interconnections

- Hashing prevents password recovery
- Salting prevents precomputed attacks
- Slow hashing prevents large-scale cracking
- Secure systems exist because attackers run out of time

---

## Conclusion

If you remember one thing from this lesson, remember this:

> Passwords are not protected by secrecy; they are protected by time.

- Hashing alone is not enough
- Salting defeats rainbow tables
- Slow hashing defeats brute-force

### Outcome achieved

You now understand how real systems defend passwords.
