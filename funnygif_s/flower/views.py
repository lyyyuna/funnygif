from django.shortcuts import render

from rest_framework import viewsets
from rest_framework.pagination import PageNumberPagination
from rest_framework.renderers import JSONRenderer
from .models import News
from .serializers import NewsSerializer


class StandardResultSetPagination(PageNumberPagination):
    page_size = 10
    page_size_query_param = 'page_size'


class NewsViewSet(viewsets.ModelViewSet):
    queryset = News.objects.all().order_by('-id')
    pagination_class = StandardResultSetPagination
    serializer_class = NewsSerializer
    renderer_classes = (JSONRenderer,)