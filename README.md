# BibleTaggingUtility
 Add and/or correct Strong's tags
 
In order to use this utility, you need to create a bible folder on your system e.g. MyBible.
The folder should contain a configuration file BiblesConfig.txt and a subfolder named tagged
The tagged subfolder should contain only one file which is the version of the tagged bible to be edited or corrected in a verse per line text file format as in the following example:
Gen 1:13 And the evening <06153> and the morning <01242> were the third <07992> day <03117>. 
Following is a sample BiblesConfig.txt

[Tagging]
taggedBible=mybible.txt

[OSIS]
osisIDWork=mybible
osisRefWork=bible
language=en
language-type=IETF
title=My Bible(Tagged)
contributor-role=ctb
contributor-name= Simon Peter
type=Bible
identifier=AraSVD-Tagged
description=This work adds Strong's references to ...
rights= ???
refSystem=Bible
ot-vpl-file=ot_tagged_text.txt
nt-vpl-file=nt_tagged_text.txt
output-file=mybible.xml

