use PIRS;

--вр. таблица 
drop table if exists #get_fios
create table #get_fios(all_FIO varchar(100) primary key clustered);

-- вр. таблица со всеми вариантами написаний всех фио
insert into #get_fios(all_FIO)
select distinct GF.all_FIO from  dbo.v_unique_authors UA cross apply dbo.get_fios2 (UA.fio) GF where GF.all_FIO is not null  

drop table if exists #main_table
create table #main_table (id int, fio_del varchar(100))

declare @desc varchar(4000)
declare @id int
declare curs cursor local fast_forward for
select 
	id, 
	description 
from 
	dbo.main_table M 
where 
	M.id<150
open curs
fetch curs into @id, @desc
WHILE @@FETCH_STATUS = 0
BEGIN
	insert into 
		#main_table
		(id, 
		fio_del)	
	select @id, F.all_FIO
	from 
		#get_fios F
	where
		patindex('%'+F.all_FIO+'%',@desc) > 0		
	fetch curs into @id, @desc
END
close curs
deallocate curs

drop table if exists #main_table_final;

select 
	row_number() over(partition by MT.id order by MT.fio_del) as row_id, MT.*, 
	convert(bit, 0) as taken
INTO 
	#main_table_final
from 
	#main_table MT 
inner join 
	dbo.main_table MT2
ON 
	MT.id = MT2.id

select * from #main_table_final

declare @row_id int = 1
while 1 = 1
begin
	update 
	MT2
	set 
		description = replace(description, MT.fio_del, '') 
	from 
		#main_table_final MT 
	inner join 
		dbo.main_table MT2
	ON 
	MT.id = MT2.id and MT.row_id = @row_id

	update #main_table_final set taken = 1 where row_id = @row_id;

	set @row_id += 1; 
	if not exists (select null from #main_table_final where taken = 0)
	break;
end
