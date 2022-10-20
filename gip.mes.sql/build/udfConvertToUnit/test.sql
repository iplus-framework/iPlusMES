-- this is test

declare @test table
(
	name varchar(20)
)

insert into @test(name) values ('a1');
insert into @test(name) values ('a2');
insert into @test(name) values ('a3');

select* from @test