# Audio
> On this page, we'll learn about the Mythril2D audio system.

The Mythril2D audio system uses a non-deterministic request-based audio playback approach. This means that scripts must issue playback requests to play music or sounds, and fulfillment depends on the current configuration. For example, if a script requests background music playback but no background music channel exists, the sound won't play and will be silently ignored.

## AudioChannel <`MonoBehaviour`>
In Mythril2D, music and sounds play through audio channels. An `AudioChannel` defines sound playback behavior: exclusive mode permits only one sound at a time, while multiple mode allows several sounds to play in parallel. Each `AudioChannel` has its own `AudioSource`, enabling different settings such as volume and pitch for each channel. For instance, you can configure the background music `AudioChannel` with lower volume than other channels. These `AudioChannels` are referenced in the `AudioSystem` (see Game Systems).

## AudioClipResolver <`DatabaseEntry`> (*Create > Mythril2D > Audio > AudioClipResolver*)
In video games, the same audio clip is often referenced in multiple locations. Updating an audio clip typically requires changes to all references of the previous clip. To address this issue and simplify audio clip selection when working with multiple clips, Mythril2D introduces the `AudioClipResolver`. This `DatabaseEntry` serves as a reference to one or more audio clips and dynamically selects one using the chosen algorithm. It also specifies which audio channel should play the clip.

## AudioRegion <`MonoBehaviour`>
Sometimes you may want to play specific sounds when the player enters certain map regions. The `AudioRegion` component achieves this by temporarily suspending background music when the player enters its trigger zone and resuming playback when the player exits.
