# Google Hash Code 2015 (qualification round)

## Summary

I tried the problem in conditions (4h to produce the best score possible) but was unable to solve a bug in time (my method to compute the intermediate score would always output 0) so I butchered a greedy solution during the last 30minutes.

The greedy solution gave a result worth **136points** (I would have needed 360points to be qualified).

## Notes

The problem could have been cut into two parts to make it more efficient to solve :
- putting servers in the datacenter to maximise the total capacity (variation of the *knapsack problem*)
- putting the servers into pools to maximise the garanted capacity (*longest task first scheduling* should give a resonable solution)

The greedy solution takes time to implement and will not bring many points but it helps debugging everything but the solver and making sure one has some points.

I lost litteraly most of my time tracking the bug, it might be easier in group were some peoples can test part of the code while other redact the parts that are still missing.

Some functions to update a single value in a list/array or get the index of the min/max/verify_predicate would have been useful.