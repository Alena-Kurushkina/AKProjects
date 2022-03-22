USE [MEI_PIRS]
GO

DECLARE @desc nvarchar(1000)='пошаговый анализ струтуры строения стенки реактора'

declare @l0 int=284
declare @l1 int=285
declare @l2 int=294
declare @l3 int=296
declare @l4 int=295

insert into dbo.tasks_table(task_desk) values (@desc);
declare @id_task int;
set @id_task=(select IDENT_CURRENT('tasks_table'));

create table #rate_authors (id_auth INT, author VARCHAR(100) , _proc int, _rate float, _concat nvarchar(500))
insert #rate_authors (id_auth, author, _proc, _rate, _concat)
execute [dbo].[get_recommend_cognates_id] @id_task;


with cte1 as
	(
		select 
			a.id_auth,
			b.author_fio, 
			0 as _proc, 
			0 as _rate, 
			null as _concat, 
			sum(c.rate)as _sum
		from 
			dbo.author_classifier a 
		inner join 
			authors b 
		on 
			a.id_auth=b.id 
		inner join 
			classifier c 
		on c.id=a.id_class
		where 
			a.id_class in (select @l0 union all select @l1 union all select @l2 union all select @l3 union all select @l4)
		group by 
			a.id_auth, b.author_fio
	),
cte2 as
	(
	select 
		id_auth, 
		author,
		_rate as gen_rate, 
		_proc,
		_rate, 
		_concat , 
		0 as _sum 
	from 
		#rate_authors
	),
cte3 as
	(
	select 
		a.id_auth 
	from 
		author_classifier a 
	inner join 
		#rate_authors b 
	on 
		a.id_auth=b.id_auth
	where 
		a.id_class in (select @l0 union all select @l1 union all select @l2 union all select @l3 union all select @l4)
	)

insert into 
	recommend_results
	(
		id_task,
		id_auth,
		_proc,
		rate,
		words,
		class_rate
	)
select
	@id_task,
	id_auth, 
	--author_fio,
	--_rate + _sum as general_rate, 
	_proc, 
	_rate, 
	_concat, 
	_sum 
from 
	cte1 
where 
	not exists (select null from cte3 where cte3.id_auth = cte1.id_auth)
	--id_auth not in (select id_auth from cte3) 

union all
select 
	@id_task,
	id_auth, 
	_proc, 
	_rate, 
	_concat , 
	_sum 
from 
	cte2 
where 
	 not exists (select null from cte3 where cte3.id_auth = cte2.id_auth)
	--id_auth not in (select id_auth from cte3)

union all
select
	@id_task,
	a.id_auth,	 
	a._proc , 
	a._rate , 
	a._concat , 
	b._sum   
from 
	cte1 b, cte2 a 
where 
	a.id_auth=b.id_auth 
	and 
	a.id_auth in (select id_auth from cte3)
--order by 
--	general_rate desc, 
--	_sum desc, 
--	_rate desc


update tasks_table set status = 1 where id = @id_task
drop table #rate_authors



