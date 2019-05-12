import random
from enum import Enum
from itertools import chain
from django.shortcuts import render
from django.core.exceptions import ObjectDoesNotExist
from django.conf import settings
from player_module.views import ensurePlayerExists, getPlayer
from utils.view_func_utils import processRequest, getErrorResponse, getSuccessResponse, convertRequestDataType, convertRequestDataTypeAll
from utils.exception import ErrorType, ErrorException
from questions.preprocess import doPreprocess
from question_module.models import Question, QuestionType, Subject, Choice, QuestionPicture
from player_module.models import Player

class QuestionDistributionType:
	Normal			=   0
	OccurFirst		=   1
	NotOccurFirst	=   2
	WorngFirst		=   3
	CorrFirst		=   4
	SimpleFirst		=   5
	MiddleFirst		=   6
	DifficultFirst	=   7

# Create your views here.
def query_id(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['id'])

		convertRequestDataTypeAll(data, 'int')

		id = data['id']+1

		data = query('id', id, 'dict')[0]

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query_ids(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['ids'])

		convertRequestDataTypeAll(data, 'int[]')

		ids = data['ids']

		for i in range(len(ids)):
			ids[i] += 1
		data = query('ids', ids, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query_all(request):
	try:
		# 获取数据
		data = processRequest(request)

		data = query('all', return_type='dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query_filter(request):
	try:
		# 获取数据
		data = processRequest(request, POST=['type', 'param'])

		convertRequestDataTypeAll(data, 'int')

		type = data['type']+1
		param = data['param']

		data = query(type, param, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query_count(request):
	try:
		# 获取数据
		data = processRequest(request)

		data = queryCount()

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def query(filter_type,  param=None, return_type='QuerySet'):
	"""
	查询题目

	:param filter_type:  过滤类型("all","id","subject","level","type","subjects")
	:param return_type:  返回类型("QuerySet","dict")
	:param param: 参数
	:return: 题目数组(Question[])
	"""
	result = Question.objects.filter(is_deleted=False)

	if filter_type == 'id':
		result = result.filter(id=param)
	elif filter_type == 'ids':
		result = result.filter(id__in=param)
	elif filter_type == 'subject':
		result = result.filter(subject_id=param)
	elif filter_type == 'subjects':
		result = result.filter(subject_id__in=param)
	elif filter_type == 'level':
		result = result.filter(level=param)
	elif filter_type == 'type':
		result = result.filter(type_id=param)

	if return_type == 'dict':
		temp = []
		for r in result:
			temp.append(r.convertToDict())
		result = temp

	return result

def queryCount():

	sbjs = Subject.objects.all()
	lv_cnt = 6

	res = []

	for sbj in sbjs:
		sbj_res = []

		for l in range(lv_cnt):
			sbj_res.append(Question.objects.filter(level=l, subject=sbj).count())
		
		res.append(sbj_res)

	return res

def generate_all(request):
	try:
		# 获取数据
		data = processRequest(request, POST=
			['sid', 'dtb_type', 'count', 'name'])

		convertRequestDataType(data, 
			['sid', 'dtb_type', 'count'], 'int')

		sid = data['sid']+1
		dtb_type = data['dtb_type']
		count = data['count']
		name = data['name']

		data = generate('all', sid, dtb_type, count, name, None, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate_type(request):
	try:
		# 获取数据
		data = processRequest(request, POST=
			['sid', 'dtb_type', 'count', 'name', 'type'])

		convertRequestDataType(data, 
			['sid', 'dtb_type', 'count', 'type'], 'int')

		sid = data['sid']+1
		dtb_type = data['dtb_type']
		count = data['count']
		name = data['name']
		type = data['type']+1

		data = generate('type', sid, dtb_type, count, name, type, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate_level(request):
	try:
		# 获取数据
		data = processRequest(request, POST=
			['sid', 'dtb_type', 'count', 'name', 'level'])

		convertRequestDataType(data, 
			['sid', 'dtb_type', 'count', 'level'], 'int')

		sid = data['sid']+1
		dtb_type = data['dtb_type']
		count = data['count']
		name = data['name']
		level = data['level']

		data = generate('level', sid, dtb_type, count, name, level, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate_exam(request):
	try:
		# 获取数据
		data = processRequest(request, POST=
			['sids', 'dtb_type', 'level_dtb', 'name'])

		convertRequestDataType(data, ['sids', 'level_dtb'], 'int[]')
		convertRequestDataType(data, ['dtb_type'], 'int')

		print (data)

		sids = data['sids']
		dtb_type = data['dtb_type']
		level_dtb = data['level_dtb']
		name = data['name']

		for i in range(len(sids)):
			sids[i] += 1

		data = generateExam(sids, dtb_type, level_dtb, name, 'dict')

	except ErrorException as exception:
		return getErrorResponse(exception)

	# 返回成功信息
	dict = {'data': data}
	return getSuccessResponse(dict)

def generate(generate_type, sid, distribute_type, 
	count, name, param=None, return_type='QuerySet'):
	"""
	生成题目

	:param generate_type:  生成类型("all","type","level","exam")
	:param sid: 科目代号
	:param distribute_type:  分配类型(Enum)
	:param count: 数量
	:param name: 玩家名称
	:param param: 附加参数
	:param return_type:  返回类型("QuerySet","dict")

	:return: 题目数组(Question[])
	"""
	player = getPlayer(name)

	questions, ignore_level = processFilter([sid], generate_type, param)

	sub = processDistribute(player, questions, 
		distribute_type, ignore_level, sid)

	return dealGenerate(sub, questions, count, return_type)

def generateExam(sids, distribute_type, 
	level_distribute, name, return_type='QuerySet'):
	"""
	生成考试题目

	:param sids: 科目代号数组
	:param distribute_type:  分配类型(Enum)
	:param level_distribute: 题目分配
	:param name: 玩家名称
	:param return_type:  返回类型("QuerySet","dict")

	:return: 题目字典({Question[]})
	"""
	player = getPlayer(name)

	questions, ignore_level = processFilter(sids, 'exam')

	sub = processDistribute(player, questions, 
		distribute_type, ignore_level)

	result = []

	for sid in sids:

		sub_result = []

		sub_sbj = sub.filter(subject_id=sid)
		questions_sbj = questions.filter(subject_id=sid)

		for i in range(len(level_distribute)):
			count = level_distribute[i]
			if count <= 0:
				continue
			sub_sbj_lv = sub_sbj.filter(level=i)
			questions_sbj_lv = questions_sbj.filter(level=i)
			sub_result.extend(dealGenerate(sub_sbj_lv, 
				questions_sbj_lv, count, return_type))

		result.append(sub_result)

	return result

def processFilter(sids, generate_type, param=None):

	questions = query('subjects', sids)

	ignore_level = False

	if generate_type == 'type':
		#type = QuestionType.objects.get(id=param)
		questions = questions.filter(type_id=param)
	elif generate_type == 'level':
		questions = questions.filter(level=param)
		ignore_level = True
	elif generate_type == 'exam':
		#questions = questions.filter(level__in=param)
		ignore_level = True

	return questions, ignore_level

def processDistribute(player, questions, distribute_type, ignore_level, sid=None):

	if not ignore_level :
		if sid != None:
			value = player.getSubjectValue(sid)
			max_level = getMaxLevel(value)

			print("max_level = "+str(max_level))
			print("value = "+str(value))

		else: # 考试模式
			max_level = len(settings.GAME_SETTING['QUESTION']['EntryValue'])
		
	#distribute_type = QuestionDistributionType(distribute_type)
	
	print("distribute_type = "+str(distribute_type))

	if not ignore_level and distribute_type >= QuestionDistributionType.SimpleFirst:
		# 难度模式
		min_level = 0
		if distribute_type == QuestionDistributionType.SimpleFirst:
			max_level = int(max_level/2)
		elif distribute_type == QuestionDistributionType.MiddleFirst:
			min_level = int(max_level/4)
			max_level = int(max_level/4*3)
		elif distribute_type == QuestionDistributionType.DifficultFirst:
			min_level = int(max_level/2)

		print(str(min_level)+" , "+str(max_level))

		sub = questions.filter(level__in=range(min_level,max_level))

	elif not ignore_level and distribute_type == QuestionDistributionType.Normal:
		sub = questions.filter(level__in=range(0,max_level))

	elif distribute_type >= QuestionDistributionType.Normal:
		# id 限制
		id_limit = []
		exclude = False
		if distribute_type == QuestionDistributionType.OccurFirst:
			id_limit = player.getDoneRec()
		elif distribute_type == QuestionDistributionType.NotOccurFirst:
			id_limit = player.getDoneRec()
			exclude = True
		elif distribute_type == QuestionDistributionType.WorngFirst:
			id_limit = player.getWrongRec()
		elif distribute_type == QuestionDistributionType.CorrFirst:
			id_limit = player.getDoneRec()
		
		print("id_limit = "+str(id_limit))

		if exclude:
			sub = questions.exclude(id__in=id_limit)
		else:
			sub = questions.filter(id__in=id_limit)
		
		if distribute_type == QuestionDistributionType.CorrFirst:
			sub = sub.exclude(id__in=player.getWrongRec())

	return sub

def dealGenerate(sub, questions, count, return_type='QuerySet'):
	result = []

	sub_len = sub.count()
	ques_len = questions.count()

	shuffleQuestion(sub)
	for i in range(count):

		question = None
		if i < sub_len:
			question = sub[i]
			print("in-condition:"+str(question.id))
		else:
			ltd = settings.GAME_SETTING['QUESTION']['GenerateTimesLimit']
			index = random.randint(0,ques_len-1)
			question = questions[index]
			while ltd > 0 and question in result:
				index = random.randint(0,ques_len-1)
				question = questions[index]
				ltd -= 1

		if return_type == 'QuerySet':
			result.append(question)
		elif return_type == 'dict':
			result.append(question.convertToDict())

	return result

def shuffleQuestion(questions):

	result = []
	for question in questions:
		result.append(question)
	random.shuffle(result)
	return result

def getMaxLevel(value):

	level = -1
	values = settings.GAME_SETTING['QUESTION']['EntryValue']
	for val in values:
		if value < val: 
			return level
		level += 1

	return level

if settings.HTML_TEST:

	def test_load(request):
		try:
			# 获取数据
			data = processRequest(request, POST=[])

			pushLocalQuestions()

		except ErrorException as exception:
			return getErrorResponse(exception)

		# 返回成功信息
		return getSuccessResponse()


	def test_randomly(request):
		try:
			# 获取数据
			data = processRequest(request, POST=['count'])

			convertRequestDataTypeAll(data, 'int')

			count = data['count']

			pushRandomlyQuestions(count)

		except ErrorException as exception:
			return getErrorResponse(exception)

		# 返回成功信息
		return getSuccessResponse()

	def test_delete(request):
		try:
			# 获取数据
			data = processRequest(request)

			deleteTestQuestions()

		except ErrorException as exception:
			return getErrorResponse(exception)

		# 返回成功信息
		return getSuccessResponse()

def pushLocalQuestions():
	questions = doPreprocess()

	for q in questions:
		choices = []
		pictures = []

		question = Question()
		question.title = q['title']
		question.level = q['level']
		question.score = q['score']
		question.subject = Subject.objects.get(id=q['subjectId']+1)
		question.type = QuestionType.objects.get(id=q['type']+1)
		question.save()

		for c in q['choices']:
			choice = Choice()
			choice.text = c['text']
			choice.answer = c['ans']
			choice.question = question
			choice.save()

		for p in q['pictures']:
			picture = QuestionPicture()
			picture.file = p
			picture.question = question
			picture.save()

def pushRandomlyQuestions(count):
	scnt = 9
	lcnt = 6
	qid = 0
	for i in range(count):
		for s in range(scnt):
			for l in range(lcnt):
				qid += 1
				qtype = random.randint(1,4)
				ccnt = random.randint(3,6)
				crtid = -1
				if qtype != 2:
					crtid = random.randint(0,ccnt)
				choices = []
				question = Question()
				question.title = '测试'+str(qid)
				question.level = l
				question.subject = Subject.objects.get(id=s+1)
				question.type = QuestionType.objects.get(id=qtype)
				question.for_test = True
				question.save()

				for c in range(ccnt):
					answer = c == crtid
					if qtype == 2:
						answer = random.randint(0,1)==1
					choice = Choice()
					if answer:
						choice.text = '正确'
						choice.answer = True
					else:
						choice.text = '错误'
					choice.question = question
					choice.save()
					"""
					choices.append(choice)


				for choice in choices:
					choice.save()"""

def deleteTestQuestions():

	questions = Question.objects.filter(for_test = True)

	for question in questions:
		question.delete()