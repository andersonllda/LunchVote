LunchVote
=========

Programa Teste DBServer

o	O que vale destacar no c�digo implementado?
	
	A arquitetura implementada para backend segue padr�es DDD.
        Para o frontend foi implementado WPF com MVVM, assim � eliminada toda a l�gica das views.
	Todo o c�digo pode ser desacoplado.
	Esta arquitetura pode ser utilizada com um ORM, por exemplo o Fluent NHibernate 
        ou o Entity Model.

o	O que poderia ser feito para melhorar o sistema?
	
	Uma camada de Services para acessar os reposit�rios seria interessante para utilizar
	o backend com outra tecnologia de frontend, assim os reposit�rios n�o seriam expostos aos
        outros projetos.
	Para uma melhor arquitetura do frontend (MVVM) � interessante criar DTOs ou Wrappers
	do Dom�nio para o retorno dos dados das ViewModels para as views.
	Se necess�rio para seguir arrisca o DDD o ideal seria colocar a regra de neg�cio no dom�nio,
	por�m com um frontend aos padr�es MVVM � poss�vel tamb�m utilizar as regras apenas nas VMs.
	O ideal seria criar testes (TDD) das VMs, para testar as fun�oes,propriedades e regras.	
	
o	Algo a mais que voc� tenha a dizer

	Como quis implementar o MVVM no frontend, a l�gica dos testes ficou um pouco confusa, tamb�m
	devido a gera��o de dados fake tive que utilizar v�rios n�meros rand�micos e setar os objetos
	no dom�nio em tempo de execu��o.
	A l�gica dos testes � diferente da l�gica das VM's, caso fosse com dados reais de um BD seria
	poss�vel utilizar a mesma l�gica das VM's nos testes.
	Os crit�rios de aceita��o s�o cumpridos nos testes de uma maneira for�ada, por�m com a 
        execu��o do programa podemos comprov�-los.

