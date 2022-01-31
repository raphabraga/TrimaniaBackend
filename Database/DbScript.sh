#!/bin/bash
echo "*** Setting Databases ***"
mysql -u root -proot < /Database/DbSetup.sql
#mysql -u trilogo -p1234 < /Database/DbPopulate.sql

