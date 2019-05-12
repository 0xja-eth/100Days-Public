from django.conf import settings
from django.http import JsonResponse, HttpResponse
from utils.exception import ErrorType, ErrorException
import json

def processRequest(request, POST=[], GET=[], FILES=[]):

	data = dict()

	for key in POST:
		value = request.POST.get(key)
		if value:
			data[key] = value
		else:
			raise ErrorException(ErrorType.ParameterError)

	for key in GET:
		value = request.GET.get(key)
		if value:
			data[key] = value
		else:
			raise ErrorException(ErrorType.ParameterError)

	for key in FILES:
		value = request.FILES.get(key)
		if value:
			data[key] = value
		else:
			raise ErrorException(ErrorType.ParameterError)

	#print (data)

	return data

def convertRequestDataType(data, keys, type='str'):
	for key in keys:
		try:
			if   type == 'int':
				data[key] = int(data[key])

			elif type == 'int[]':
				data[key] = json.loads(data[key])
				for i in range(len(data[key])):
					data[key][i] = int(data[key][i])
			
			elif type == 'save':
				pass
				# json.loads(data[key])
				# 判断是否为存档文件类型
			# 其他类型判断

		except:
			raise ErrorException(ErrorType.ParameterError)

def convertRequestDataTypeAll(data, type='str'):
	convertRequestDataType(data, data, type)

def getSuccessResponse(dict={}):
	#if 'data' in dict:
	processData(dict) # json.dumps(dict['data'],ensure_ascii=False)
	
	dict['status'] = ErrorType.Success.value

	#print (dict)

	if settings.HTML_TEST:
		# 测试代码
		response = JsonResponse(dict)
		response["X-Frame-Options"] = ''

		return response
	else:
		return JsonResponse(dict)

def processData(data):
	if type(data) == list:
		for i in range(len(data)):
			processData(data[i])
			if type(data[i]) == list:
				data[i] = {'list': data[i]}
	elif type(data) == dict:
		for key in data:
			processData(data[key])
			if type(data[key]) == list:
				data[key] = {'list': data[key]}

def getErrorResponse(exception: ErrorException):
	dict = {
		'status': exception.error_type.value,
		'errmsg': str(exception)
	}

	if settings.HTML_TEST:
		# 测试代码
		response = JsonResponse(dict)
		response["X-Frame-Options"] = ''

		return response
	else:
		return JsonResponse(dict)
	