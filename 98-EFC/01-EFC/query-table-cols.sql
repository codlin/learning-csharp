USE [AdventureWorks2022];
GO
select sc.name, st.name
from syscolumns sc, systypes st
where sc.xtype=st.xtype and sc.id in(select id
    from sysobjects
    where xtype='U' and name='Product');
GO