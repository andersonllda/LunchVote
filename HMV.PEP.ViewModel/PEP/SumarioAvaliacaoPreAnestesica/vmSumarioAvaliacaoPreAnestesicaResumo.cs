using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Expression;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica
{
    public class vmSumarioAvaliacaoPreAnestesicaResumo
    {
        public vmSumarioAvaliacaoPreAnestesicaResumo(wrpSumarioAvaliacaoPreAnestesica pSumarioAvaliacaoPreAnestesica)
        {
            SumarioAvaliacaoPreAnestesica = pSumarioAvaliacaoPreAnestesica;
        }

        public wrpSumarioAvaliacaoPreAnestesica SumarioAvaliacaoPreAnestesica { get; set; }

        public string IdadeDoPaciente
        {
            get { return SumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.Paciente.Idade.GetDate(); }
        }

        public string SexoDoPaciente
        {
            get { return SumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.Paciente.Sexo.ToString(); }
        }

        public string DataCirurgia
        {
            get { return SumarioAvaliacaoPreAnestesica.AvisoCirurgia.Atendimento.Paciente.Sexo.ToString(); }
        }

        public string CID
        {
            get { return SumarioAvaliacaoPreAnestesica.CID == null ? string.Empty : SumarioAvaliacaoPreAnestesica.CID.Descricao.Combine(" - ").Combine(SumarioAvaliacaoPreAnestesica.CID.Id); }
        }

        public string DataHoraCirurgia
        {
            get
            {
                if (this.SumarioAvaliacaoPreAnestesica.AvisoCirurgia == null || this.SumarioAvaliacaoPreAnestesica.AvisoCirurgia.dt_aviso_cirurgia == null)
                    return string.Empty;
                return this.SumarioAvaliacaoPreAnestesica.AvisoCirurgia.dt_aviso_cirurgia.ToString("dd/MM/yyyy - HH:mm");
            }
        }

        public string CorPaciente
        {
            get
            {
                if (this.SumarioAvaliacaoPreAnestesica.AvisoCirurgia == null)
                    return string.Empty;
                return this.SumarioAvaliacaoPreAnestesica.AvisoCirurgia.Paciente.Cor.HasValue ? this.SumarioAvaliacaoPreAnestesica.AvisoCirurgia.Paciente.Cor.Value.ToString() : string.Empty;
            }
        }

        public List<wrpSumarioAvaliacaoPreAnestesicaItem> ListaRelatorios(int ord)
        {
            return SumarioAvaliacaoPreAnestesica.SumarioAvaliacaoPreAnestesicaItens.Where(x => x.AnestesiaItem.AnestesiaGrupo.OrdemRelatorio == ord).ToList();
        }

        public List<SumarioDTO> SumarioDTOCollection { get; set; }

        public class SumarioDTO
        {
            public virtual string Titulo { get; set; }
            public virtual string Item { get; set; }
            public virtual string Descricao { get; set; }
            public virtual string Observacao { get; set; }
            public virtual string Valor { get; set; }
        }
    }
}
