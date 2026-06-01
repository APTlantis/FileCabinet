# FileCabinet — The Deliberate Retention Tradeoff

FileCabinet is built around a simple tradeoff.

You do a little more work when an artifact enters the vault so you can get much more value from it later. That work might be choosing copy or move mode, checking the generated category, adding tags, writing a retention reason, recording provenance, or marking the trust state. None of that is as effortless as dropping a file into Downloads and walking away.

That is intentional.

## Why The Extra Step Matters

Files are easiest to understand when they are fresh.

At the moment you save an installer, manifest, recovery key, dataset, image, archive, or configuration file, you often know:

- where it came from
- why you kept it
- whether you trust it
- what project, device, release, or incident it belongs to
- whether it is temporary, important, or evidence-grade
- what would be painful if you had to rediscover it later

Six months later, that context is weaker. Years later, it may be gone.

FileCabinet asks for enough structure to preserve that context before it decays.

## Effort Scales The Payoff

The vault does not require perfection to be useful.

Even a lightly reviewed artifact with a generated category, a few basic tags, stored hashes, original path, and extracted text already has more long-term context than a loose file sitting in a folder. A minimal record can still help you search, verify, repair, relate, and recover the artifact later.

But the richer the record, the stronger the future payoff.

When you use the fields FileCabinet already provides, the artifact becomes much harder to lose:

- retention reason explains why it was kept
- why this matters preserves significance
- source provenance records where it came from
- acquisition method explains how it entered your possession
- trust classification captures confidence
- retention priority separates routine files from critical ones
- archive status shows whether it is active, archived, quarantined, or waiting for review
- notes preserve details that do not fit a structured field

Future roadmap work should keep expanding that ladder of context while making it easier to climb. Starter values, intake profiles, bulk tools, review queues, and better prompts can reduce friction, but the principle stays the same: the more useful context you put in while the artifact is fresh, the richer and more resilient the vault becomes later.

FileCabinet can still reward a quick pass. It rewards a careful pass much more.

## Not Every File Belongs Here

FileCabinet is not trying to replace the normal filesystem.

Downloads, project folders, scratch directories, and ordinary document folders are still useful. They are fast, loose, and appropriate for daily work. FileCabinet is for the smaller set of files where future trust matters more than immediate convenience.

Good candidates are high-signal artifacts:

- installers that may disappear from vendor sites
- drivers and firmware that fixed a real problem
- manifests, logs, and configuration snapshots
- keys, recovery files, and operational documents
- datasets and research artifacts
- release packages and build evidence
- archives that need provenance and verification

The vault is not a junk drawer. It is a deliberate retention space.

## Making The Work Easier

The goal is not to make operators suffer through forms.

FileCabinet should keep reducing friction:

- starter tags
- inferred categories
- searchable extracted text
- bulk metadata tools
- intake profiles
- saved views
- review queues
- repair suggestions
- CLI reporting

Those features make the up-front work lighter and more repeatable. They should guide the operator through good preservation habits without pretending the operator's judgment is unnecessary.

The best version of FileCabinet does not remove the human from preservation. It helps the human spend attention where it matters.

## The Payoff

The reward comes later.

When a retained artifact is needed again, FileCabinet should help answer:

- What is this?
- Where did it come from?
- Why did I keep it?
- Can I trust it?
- Has it changed?
- What is related to it?
- How do I restore or export it?

That is the value of doing a little work up front. The vault turns a file from something you merely saved into something you can understand, verify, and use again. The more context you preserve at intake and review time, the less likely the artifact is to become anonymous, mistrusted, or effectively lost later.

For the right person and the right artifacts, that tradeoff is worth it.
