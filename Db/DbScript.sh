#!/bin/bash
echo "*** Setting Databases ***"
mysql -u root -proot < /Db/DbSetup.sql
#mysql -u trilogo -p1234 < /Db/DbPopulate.sql

