using LunchVote.Core;
using LunchVote.Core.ViewModelBaseClasses;
using LunchVote.Domain;
using LunchVote.Domain.Repository;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LunchVote.VM
{
    public class VMVotacao : ViewModelBase
    {
        private Profissional _profissional;
        private Votacao _votacao;
        private Restaurante _RestauranteOpcao1;
        private Restaurante _RestauranteOpcao2;
        private Restaurante _RestauranteOpcao3;
        private bool _RestauranteOpcao1Value;
        private bool _RestauranteOpcao2Value;
        private bool _RestauranteOpcao3Value;
        private string _resultado;
        private bool _mostraresultado;
        private bool _mostrabotaoresultado;
        private bool _mostrabotaovotar;

        IRepositoryProfissional repprofissional = ObjectFactory.GetInstance<IRepositoryProfissional>();
        IRepositoryVotacao repvotacao = ObjectFactory.GetInstance<IRepositoryVotacao>();
        //IRepositoryRestaurante represtaurante = ObjectFactory.GetInstance<IRepositoryRestaurante>();

        public VMVotacao()
        {
            _profissional = repprofissional.OndeIdIgual(new Random().Next(10)).Single();
            _carregaVotacao();
            _carregaOpcoes();
            _carregaVotosFake();
            _carregaResultado();

            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 0, 0))
            {
                if (MessageBox.Show("Já passou do meio dia, deseja testar a votação?", "Teste", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    _mostraresultado = true;
                    _mostrabotaoresultado = false;
                    _mostrabotaovotar = false;
                }
                else
                {
                    _mostrabotaoresultado = true;
                    _mostrabotaovotar = true;
                }
            }
            else
            {
                _mostrabotaoresultado = true;
                _mostrabotaovotar = true;
            }            
        }

        private void _carregaResultado()
        {
            var counts = _votacao.Votos.GroupBy(x => x.Restaurante)
                                      .ToDictionary(g => g.Key,
                                                    g => g.Count()).OrderByDescending(x => x.Value);
            var maisvotado = counts.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            _resultado = "Restaurante          Votos\r\n";
            foreach (var item in counts)
                _resultado += item.Key.Descricao + "            " + item.Value + "\r\n";

            OnPropertyChanged<VMVotacao>(x => x.Resultado);
        }

        private void _carregaVotosFake()
        {
            Random r = new Random();
            for (int i = 0; i <= 29; i++)
            {
                var rep = ObjectFactory.GetInstance<IRepositoryVoto>();
                var v = new Voto() { Id = i, Profissional = _profissional };
                v.Votacao = _votacao;
                var id = r.Next(0, 3);
                v.Restaurante = _votacao.Restaurantes[id];
                rep.Save(v);
                _votacao.Votos.Add(v);
            }
        }

        private void _carregaVotacao()
        {
            _votacao = repvotacao.OndeDataIgual(DateTime.Now.Date).Single();
            _votacao.Profissional = ObjectFactory.GetInstance<IRepositoryProfissional>().OndeIsFacilitador().List().FirstOrDefault();
            
            Random r = new Random();
            List<Restaurante> restaurantesganhadores = new List<Restaurante>();
            var diasdasemana = DateTime.Now.Date.GetDaysInWeek(DayOfWeek.Monday).Take(5).ToList();
            foreach (var dia in diasdasemana)
            {
                var votacao = ObjectFactory.GetInstance<IRepositoryVotacao>().OndeDataIgual(dia).Single();
                votacao.RestauranteMaisVotado = ObjectFactory.GetInstance<IRepositoryRestaurante>().OndeIdIgual(r.Next(1,20)).Single();
                restaurantesganhadores.Add(votacao.RestauranteMaisVotado);
            }

            while (_votacao.Restaurantes.Count() < 3)
            {
                var rest = ObjectFactory.GetInstance<IRepositoryRestaurante>().OndeIdIgual(r.Next(1,20)).Single();
                if (restaurantesganhadores.Count(x => x.Id == rest.Id) == 0
                    && _votacao.Restaurantes.Count(x=> x.Id == rest.Id) == 0)
                    _votacao.Restaurantes.Add(rest);
            }
            //_votacao.Restaurantes.Add(ObjectFactory.GetInstance<IRepositoryRestaurante>().OndeIdIgual(r.Next(1, 3)).Single());
            //_votacao.Restaurantes.Add(ObjectFactory.GetInstance<IRepositoryRestaurante>().OndeIdIgual(r.Next(4, 7)).Single());
            //_votacao.Restaurantes.Add(ObjectFactory.GetInstance<IRepositoryRestaurante>().OndeIdIgual(r.Next(7, 10)).Single());
        }

        private void _carregaOpcoes()
        {
            _RestauranteOpcao1 = _votacao.Restaurantes[0];
            _RestauranteOpcao2 = _votacao.Restaurantes[1];
            _RestauranteOpcao3 = _votacao.Restaurantes[2];
        }

        public Restaurante RestauranteOpcao1
        {
            get { return _RestauranteOpcao1; }
        }
        public Restaurante RestauranteOpcao2
        {
            get { return _RestauranteOpcao2; }
        }
        public Restaurante RestauranteOpcao3
        {
            get { return _RestauranteOpcao3; }
        }

        public bool RestauranteOpcao1Value
        {
            get { return _RestauranteOpcao1Value; }
            set
            {
                _RestauranteOpcao1Value = value;
                _RestauranteOpcao2Value = !value;
                _RestauranteOpcao3Value = !value;

                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao1Value);
                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao2Value);
                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao3Value);
            }
        }

        public bool RestauranteOpcao2Value
        {
            get { return _RestauranteOpcao2Value; }
            set
            {
                _RestauranteOpcao2Value = value;
                _RestauranteOpcao1Value = !value;
                _RestauranteOpcao3Value = !value;

                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao1Value);
                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao2Value);
                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao3Value);
            }
        }

        public bool RestauranteOpcao3Value
        {
            get { return _RestauranteOpcao3Value; }
            set
            {
                _RestauranteOpcao3Value = value;
                _RestauranteOpcao1Value = !value;
                _RestauranteOpcao2Value = !value;

                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao1Value);
                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao2Value);
                OnPropertyChanged<VMVotacao>(x => x.RestauranteOpcao3Value);
            }
        }

        public string Resultado
        {
            get
            {
                return _resultado;
            }
        }

        public string ResultadoBotaoTexto
        {
            get
            {
                if (!_mostraresultado)
                    return "Mostra Resultado";

                return "Esconde Resultado";
            }
        }
        public Visibility IsResultadoVisivel
        {
            get
            {
                if (_mostraresultado)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }
        public Visibility IsBotaoResultadoVisivel
        {
            get
            {
                if (_mostrabotaoresultado)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        public Visibility IsBotaoVotarVisivel
        {
            get
            {
                if (_mostrabotaovotar)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }
        public string VotacaodoDia
        {
            get
            {
                if (_mostrabotaovotar)
                    return "Votação do dia: " + _votacao.Data.ToShortDateString() + " encerra ao meio dia.";
                else
                    return "Votação do dia: " + _votacao.Data.ToShortDateString() + " encerrada!";
            }
        }

        protected override bool CommandCanExecuteVotar(object param)
        {
            return (_RestauranteOpcao1Value || _RestauranteOpcao2Value || _RestauranteOpcao3Value);
        }
        protected override void CommandVotar(object param)
        {
            Restaurante restaurantevotado = null;
            if (_RestauranteOpcao1Value)
                restaurantevotado = _RestauranteOpcao1;
            if (_RestauranteOpcao2Value)
                restaurantevotado = _RestauranteOpcao2;
            if (_RestauranteOpcao3Value)
                restaurantevotado = _RestauranteOpcao3;
            var voto = new Voto() { Profissional = _profissional, Restaurante = restaurantevotado, Votacao = _votacao };
            IRepositoryVoto rep = ObjectFactory.GetInstance<IRepositoryVoto>();
            rep.Save(voto);
            _votacao.Votos.Add(voto);
            IRepositoryVotacao repv = ObjectFactory.GetInstance<IRepositoryVotacao>();
            repv.Save(_votacao);

            _carregaResultado();
            _mostraresultado = true;
            _mostrabotaoresultado = false;
            _mostrabotaovotar = false;
            OnPropertyChanged<VMVotacao>(x => x.IsResultadoVisivel);
            OnPropertyChanged<VMVotacao>(x => x.IsBotaoResultadoVisivel);
            OnPropertyChanged<VMVotacao>(x => x.IsBotaoVotarVisivel);           
        }

        protected override void CommandVisualizar(object param)
        {
            if (_mostraresultado)
                _mostraresultado = false;
            else
                _mostraresultado = true;

            OnPropertyChanged<VMVotacao>(x => x.IsResultadoVisivel);
            OnPropertyChanged<VMVotacao>(x => x.ResultadoBotaoTexto);
        }        
    }
}
