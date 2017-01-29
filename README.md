# Google Hash Code 2015 (qualification round)

## Summary

I tried the problem in conditions (4h to produce the best score possible) adapting a personnal algorithm that solves the knapsack problem but was unable to correct a bug in time (my method to compute the intermediate score would always output 0) so I butchered a greedy solution during the last 30minutes.

My quick greedy algorithm gives pools at random and insert the servers as tightly as possible from the bigger to the smaller. It gave a result worth **136points** (I would have needed 360points to be qualified).

## Extended round 

Instead of searching for the best combinaison of pool and row per server at once, one can cut the problem into two parts to make it easier to solve :
- putting servers in the datacenter to maximise the total capacity
- putting the servers into pools to maximise the garanted capacity

The best approach to maximise the total capacity is probably to rely on a variant of the *knapsack problem* but here I will still try to implement a solution in a short time (while still not peeking at other participants solutions).

Selecting pools using an approach akind to *longest task first scheduling* was enough to get **314points**.

Changing the criteria for the greedy algorithm from *bigger first* to *(most_efficient,bigger) first* got me to **371points** (enough to get in the final round).

## Notes

The greedy solution takes time to implement and is far from optimal but it helps debugging everything and, at least, gives a solution.

I lost litteraly most of my time tracking the bug, it might be easier in group were some peoples can test part of the code while other redact the parts that are still missing.

Some functions to update a single value in a list/array or get the index of the min/max/verify_predicate would have been useful.

## Solutions 

I found a [post](https://zestedesavoir.com/forums/sujet/2317/google-hash-code-edition-2015/?page=2) on a forum saying that two selected teams used a similar greedy algorithm. It appears to be the main approach to solved that problem.