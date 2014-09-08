LunchVote
=========

Programa Teste DBServer

o	O que vale destacar no código implementado?
	
	A arquitetura implementada para backend segue padrões DDD.
        Para o frontend foi implementado WPF com MVVM, assim é eliminada toda a lógica das views.
	Todo o código pode ser desacoplado.
	Esta arquitetura pode ser utilizada com um ORM, por exemplo o Fluent NHibernate 
        ou o Entity Model.

o	O que poderia ser feito para melhorar o sistema?
	
	Uma camada de Services para acessar os repositórios seria interessante para utilizar
	o backend com outra tecnologia de frontend, assim os repositórios não seriam expostos aos
        outros projetos.
	Para uma melhor arquitetura do frontend (MVVM) é interessante criar DTOs ou Wrappers
	do Domínio para o retorno dos dados das ViewModels para as views.
	Se necessário para seguir arrisca o DDD o ideal seria colocar a regra de negócio no domínio,
	porém com um frontend aos padrões MVVM é possível também utilizar as regras apenas nas VMs.
	O ideal seria criar testes (TDD) das VMs, para testar as funçoes,propriedades e regras.	
	
o	Algo a mais que você tenha a dizer

	Como quis implementar o MVVM no frontend, a lógica dos testes ficou um pouco confusa, também
	devido a geração de dados fake tive que utilizar vários números randômicos e setar os objetos
	no domínio em tempo de execução.
	A lógica dos testes é diferente da lógica das VM's, caso fosse com dados reais de um BD seria
	possível utilizar a mesma lógica das VM's nos testes.
	Os critérios de aceitação são cumpridos nos testes de uma maneira forçada, porém com a 
        execução do programa podemos comprová-los.

