USE [MEI_PIRS]
GO


create PROCEDURE [dbo].[get_recommend_cognates_id] 
(
	@id_task int
)
AS
BEGIN

	--declare @desc nvarchar(1000)='Описание задачи для подбора исполнителей';
	--insert into dbo.tasks_table(task_desk) values (@desc);
	--declare @id_task int;
	--set @id_task=(select IDENT_CURRENT('tasks_table'));

	--получаем описание задачи, очищаем от лишнего
	declare @desc nvarchar(1000) = (select task_desk from tasks_table where id=@id_task)
	declare @id_table int
	set @id_table = (select table_id from dbo.used_tables where id = 4);
	select @desc = dbo.delete_after_sleshes(@desc)
	exec dbo.clean_fio_str @desc = @desc, @out = @desc output
	select @desc = dbo.clear_with_reg_expr(@desc)

	--делим на слова
	create table #vrem (word nvarchar(200))
	insert 
		#vrem (word) 
	select 
		value 
	from 
		dbo.uf_inline_split_me_pirs(@desc,'')

	declare @id_word int;
	DECLARE @vword nvarchar(1000);

	--проходим по всем словам
	declare word_curs CURSOR FOR 
		SELECT 
			word 
		from 
			#vrem;
	open word_curs;
	FETCH NEXT FROM word_curs INTO @vword; 
	WHILE @@fetch_status = 0
		BEGIN		
		if (len(@vword)>3)
		begin
			select @id_word = id FROM dbo.dict WHERE word_nf = @vword; 
			if (@id_word is NULL)--есть ли такое слово в словаре
				begin
				insert into dbo.dict (word_nf) 
					select @vword;
				set @id_word=(select IDENT_CURRENT('dict'));
				end
			insert into dbo.words_step2(id_record, id_table, id_word_nf)
				select @id_task, @id_table, @id_word
			set @id_word = NULL
		end
		FETCH NEXT FROM word_curs INTO @vword; 
		END
	CLOSE word_curs;
	DEALLOCATE word_curs;
		
		create table #timetable (
		--id_task int,
		id_author int,
		author_fio nvarchar (100),
		word_nf nvarchar(100),
		id_word int,
		count_w int,
		koef float,
		cognates_flag int default 0
		)

		create table #tmtable (
		id_dict int	
		)

		declare @id_main_table int;
		set @id_main_table = (select table_id from dbo.used_tables where id=1)
		declare @id_tasks_table int;
		set @id_tasks_table = (select table_id from dbo.used_tables where id=4);

		with m as 
		(
			select 
				id_trimmed 
			from 
				relation_t 
			where 
				id_dict in 
				(
					select 
						id_word_nf 
					from 
						dbo.words_step2 
					where 
						id_record=@id_task 
						and 
						id_table = @id_tasks_table
				)
		)
		insert into 
			#tmtable
		select 
			t.id_dict as word_from_dict
		from 
			m 
		inner join
			relation_t t
		on 
			m.id_trimmed=t.id_trimmed

		insert into 
			#timetable (
				id_author,
				author_fio,
				word_nf,
				id_word,
				count_w, 
				koef
			)
		select 
			--w.id_record,
			w.id_author,
			a.author_fio,
			d.word_nf,
			w.id_word_nf,
			d.koef,
			w.count
		from 
			dbo.words_step2 w, 
			dbo.authors a, 
			dbo.dict d
		where 
			a.id=w.id_author 
			and 
			d.id=w.id_word_nf 
			and
			w.id_table=@id_main_table 
			and w.id_word_nf in 
				(
				select 
					id_dict 
				from 
					#tmtable
				);

		update 
			#timetable 
		set 
			cognates_flag = 1 
		where 
			id_word in 
				(
				select 
					id_word_nf 
				from 
					dbo.words_step2 
				where 
					id_record=@id_task
					and
					id_table = @id_tasks_table
				) 

		declare @max_w int;		
		set @max_w = (select count(distinct word_nf) from #timetable);
		
		with cte as 
		(
		select 
			id_author, 
			dbo.GROUP_CONCAT_D(word_nf,', ') as _concat 
		from 
			#timetable 
		where 
			cognates_flag=1 
		group by 
			id_author
		),
		cte1 as
		(
		select 
			id_author, 
			author_fio as _author, 
			count(id_word)*100/@max_w as _proc, 
			sum(count_w*koef) as _rate 
		from 
			#timetable 
		group by 
			id_author, 
			author_fio
		)
		select 
			c1.id_author, 
			c1._author, 
			c1._proc, 
			c1._rate, 
			c._concat 
		from 
			cte c 
		join 
			cte1 c1 
		on 
			c.id_author=c1.id_author
		order by 
			c1._proc desc, 
			c1._rate desc
				
		--select t.id_author, author_fio, count(id_word)*100/@max_w as _proc, sum(count_w) as _rate, dbo.GROUP_CONCAT_D(word_nf,', ')
		--from #timetable t 
		--group by t.id_author, author_fio 
		--order by _proc desc, _rate desc				
		
		drop table #timetable;
		drop table #tmtable;
		drop table #vrem;

end


--DECLARE @RC int
--DECLARE @desc1 nvarchar(1000)='Описание задачи для подбора исполнителей'
--EXECUTE @RC = [dbo].[get_recommend_cognates] 
--   @desc1

GO



