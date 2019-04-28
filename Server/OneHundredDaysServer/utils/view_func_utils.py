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

	return data

def convertRequestDataType(data, key, type='str'):
	try:
		if   type == 'int':
			data[key] = int(data[key])
		
		elif type == 'video':
			pass # 判断是否为视频类型
		# 其他类型判断

	except ErrorException as exception:
		raise ErrorException(ErrorType.ParameterError)

def getSuccessResponse(dict={}):
	dict['status'] = ErrorType.Success.value

	if settings.HTML_TEST:
		# 测试代码
		response = JsonResponse(dict)
		response["X-Frame-Options"] = ''

		return response
	else:
		return JsonResponse(dict)



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
	