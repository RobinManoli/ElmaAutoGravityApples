Download:
https://github.com/RobinManoli/ElmaAutoGravityApples/raw/main/ElmaAutoGravityApples.zip

Discuss:
https://mopolauta.moposite.com/posting.php?mode=quote&p=240512

Test Level (for lgr):
https://elma.online/levels/574599

# ElmaAutoGravityApples
For this to work you need to download the zip file above, run the program, open your level in the program. Then your level is fixed. You also need to have a compatible LGR (included in the .zip).

Backup your level before trying anything.

Select a lev with gravity apples.

All apples will change their  food anim number,
so that they can display specific apples
for every gravity type.

Make sure to use an lgr for this level that has:
qfood1.pcx as norm apple (no gravity),
qfood2.pcx as gravity up,
qfood3.pcs as gravity down,
qfood4.pcx as gravity left,
qfood5.pcx as gravity right.

# Technical info
// no gravity apple: gravityType == 0 // anim number stored value == 0 // anim number displayed value in elma editor == 1 // qfood1.pcx
// gravity up: gravityType == 1 // anim number stored value == 1 // anim number displayed value in elma editor == 2 // qfood2.pcx
// gravity down: gravityType == 2 // anim number stored value == 2 // anim number displayed value in elma editor == 3 // qfood3.pcx
// gravity left: gravityType == 3 // anim number stored value == 3 // anim number displayed value in elma editor == 4 // qfood4.pcx
// gravity right: gravityType == 4 // anim number stored value == 4 // anim number displayed value in elma editor == 5 // qfood5.pcx
