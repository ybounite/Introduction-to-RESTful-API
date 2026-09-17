# What Brute-Force Attacks Look Like

## Introduction

![Brute-force introduction](https://uploads.teachablecdn.com/attachments/92231502220343c6b4b84b108d4a181b.png)

When developers hear "brute-force attack", many imagine:

- A human guessing passwords
- A few wrong attempts
- An impatient user

That mental model is wrong.

Modern brute-force attacks are not human actions. They are automated processes designed to exploit repetition.

## What a Real Brute-Force Attack Is

![Real brute-force attack](https://uploads.teachablecdn.com/attachments/d14d09881f3649d68dc07bc80d4596e9.png)

A real brute-force attack looks like this:

- Hundreds or thousands of login requests
- Requests sent by scripts or bots
- No pauses, no thinking
- Continuous retries

The attacker does not try harder. They simply try more.

## Common Brute-Force Patterns

![Common brute-force patterns](https://uploads.teachablecdn.com/attachments/bb94a06e9b0643199dce9c9795ae526a.png)

Attackers typically use one of these patterns:

- Same email, many passwords
- Same password, many emails
- Sequential guessing from a wordlist
- High-frequency requests, sometimes every second or faster

One request is harmless. Repetition is the weapon.

## Why Brute-Force Attacks Are Effective

![Why brute-force attacks work](https://uploads.teachablecdn.com/attachments/318120727bdb4252b32e3ee59478f47a.png)

Brute-force attacks succeed because:

- Computers are fast
- APIs respond quickly
- Requests are cheap to send
- Time favors the attacker

If attempts are unlimited, the attacker eventually wins.

## Characteristics

- Fully automated
- High request volume
- Repetitive behavior
- No concern for errors
- Exploits unlimited retries

## Conclusion

Brute-force attacks are not about guessing well. They are about guessing endlessly.

Any authentication system that allows unlimited attempts is vulnerable by design.

## How Attackers Abuse Login Endpoints

![Login endpoint abuse](https://uploads.teachablecdn.com/attachments/3cd9960abb1c4846af0f70e071b2d5bb.png)

## Introduction

Login endpoints are the most abused endpoints in any API.

Not because they are weak, but because they are valuable and exposed.

## Why Login Endpoints Are Targeted

Attackers focus on login endpoints because:

- They accept secrets such as passwords and tokens
- They are publicly accessible
- They return immediate success or failure
- They can be called without authentication

Breaking login breaks everything.

## How Abuse Happens in Practice

![Login abuse in practice](https://uploads.teachablecdn.com/attachments/8a53ddc24c094ebbadd962f3a720e0fc.png)

Attackers abuse login endpoints by:

- Sending rapid repeated requests
- Testing leaked credentials automatically
- Flooding the endpoint continuously

They do not care if responses are `401` or if errors are returned.

They care only about the one request that succeeds.

## Credential Stuffing

![Credential stuffing](https://uploads.teachablecdn.com/attachments/1abd16cba9d24a50a068d5fd6bf5aed5.png)

Credential stuffing works like this:

- Attackers obtain leaked email and password lists
- They test them automatically against your API
- They rely on password reuse

Even if 99.9% of attempts fail, the remaining 0.1% is enough at scale.

## Characteristics

- Abuse is automated
- Login endpoints are cheap to attack
- Feedback is immediate
- Scale compensates for a low success rate

## Conclusion

Login endpoints are not abused once. They are abused constantly.

Any public login endpoint must assume hostile usage.

## Why Authentication Without Limits Is Dangerous

## Introduction

Many systems implement authentication correctly and stop there.

This creates a false sense of security.

Correct authentication without limits is still dangerous.

## Unlimited Retries Favor the Attacker

![Unlimited retries favor the attacker](https://uploads.teachablecdn.com/attachments/cbd5ad3f444a44a08ad12e78bfbd055a.png)

If your API allows unlimited login attempts:

- Attackers can try forever
- Automation removes effort
- Time favors the attacker

Security becomes a probability game:

> Eventually, something will work.

## Failure Still Costs Resources

![Failure still costs resources](https://uploads.teachablecdn.com/attachments/4ae71778eb2b4982a3b8c6f0448b2e87.png)

Every failed login attempt still:

- Hashes a password
- Allocates memory
- Uses CPU
- Writes logs

Even failed attempts damage system stability at scale.

## Small Leaks Become Big Breaches

![Small leaks become big breaches](https://uploads.teachablecdn.com/attachments/946b43e3ebfa40ec8cb4dfd4c4ef0971.png)

Without limits:

- One leaked password is enough
- One weak account can be compromised
- Abuse spreads quietly

Unlimited retries turn small mistakes into major incidents.

## Authentication Alone Does Not Control Behavior

![Authentication alone does not control behavior](https://uploads.teachablecdn.com/attachments/fcde3d2e8f4748f7876d5c9e6d9bd406.png)

Authentication answers:

> Who are you?

It does not answer:

> How often should you try?

Behavior must be controlled separately.

## Characteristics

- Unlimited retries favor automation
- Authentication alone does not stop abuse
- Scale amplifies small weaknesses
- Limits are mandatory, not optional

## Conclusion

Authentication without limits:

- Assumes good behavior
- Ignores automation
- Fails under real-world conditions

Secure systems control behavior, not just credentials.
