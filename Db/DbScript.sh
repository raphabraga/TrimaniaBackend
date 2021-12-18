#!/bin/bash
mysql -u root -proot < /Db/DbSetup.sql

if [ $1 = -p ] || [ $1 = --populate ]; then
    mysql -u trilogo -p1234 < /Db/DbPopulate.sql
fi
