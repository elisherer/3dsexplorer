# Introduction #

This page will show the progress made for saving altered save files.

# Progress #

  * Memory Map, Journal & Blockmap are in the context.
  * CRC16 of the blockmap is possible through CRC16.cs.


---


## Unknown Hashes ##

**Image Hash:** (0x10 @ 0x0000)

> + Changes from one save to another.

> - Algorithm is unknown.

**Partition Hash:** (0x20 @ 0x010C into Parition entry)

> + Changes from one save to another.

> ? Algorithm - Probably SHA256 becuase it's 32 Bytes.

> - Isn't understood. (didn't find with brute force)

**Partition Hash Table Header:** (0x40 @ 0x0000 into the partition)

> + Changes from one save to another.

> - Algorithm is unknown.


---

# Algorithm Proposal #

## Required stuff: ##

  * Image binary (byte array)
  * Key Binary (byte array - 512 bytes)
  * Blockmap + Journal + Memory Map
  * Disa & Partition table

After changing a file in the partition.
  1. (Done) Go over the partitions and rehash the hashtable entries (that aren't 0)
  1. (Unknwon) Make the 'Partition Hash Table Header'
  1. (Unknown) Go over the Partition table and and make the proper hash (for SAVE & DATA).
  1. (Done) Hash the active partition table into the DISA struct.
  1. (Done) Rearrange the 0x1000 blocks according to the Memory Map.
  1. (Done) Chechsum every 0x1000 block according to 3DBrew.
  1. (Done) CRC16 (Modbus) the blockmap
  1. (Done) XOR the whole thing from 0x1000 with the key-file.