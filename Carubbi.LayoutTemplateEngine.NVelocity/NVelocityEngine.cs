﻿using Carubbi.LayoutTemplateEngine.Interfaces;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Carubbi.LayoutTemplateEngine.NVelocity
{
    /// <inheritdoc />
    /// <summary>
    /// Gerenciador de templates baseado na implementação NVelocity
    /// </summary>
    public class NVelocityEngine : ILayoutTemplateEngine
    {
        private readonly string _templatesPath;

        public NVelocityEngine()
        {

        }

        public NVelocityEngine(string templatesPath)
        {
            _templatesPath = templatesPath;
        }


        public string RenderFromContentTemplate(string content, IDictionary<string, object> data)
        {
            var writer = new StringWriter();

            var e = new VelocityEngine();
            e.Init();
            var context = new VelocityContext();
            var templateData = data ?? new Dictionary<string, object>();

            foreach (var key in templateData.Keys)
            {
                context.Put(key, templateData[key]);
            }
            e.Evaluate(context, writer, "template", content);
            return writer.ToString();

        }

        private IContext CreateContext(IDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        #region ILayoutTemplateEngine Members

        /// <inheritdoc />
        /// <summary>
        /// Renderiza um template a partir do nome, substituindo as variáveis do template pelas variáveis passadas como parâmetro
        /// </summary>
        /// <param name="templateName">Nome do Template</param>
        /// <param name="data">Dicionário com as váriáveis (Chave/Valor)</param>
        /// <returns>Template com as variávies substituídas</returns>
        public string RenderTemplate(string templateName, IDictionary<string, object> data)
        {
            return RenderTemplate(null, templateName, data);
        }

        /// <inheritdoc />
        /// <summary>
        /// Renderiza um template a partir do nome utilizando masterPage, substituindo as variáveis do template pelas variáveis passadas como parâmetro
        /// </summary>
        /// <param name="masterPage">Nome da master page</param>
        /// <param name="templateName">Nome do Template</param>
        /// <param name="data">Dicionário com as váriáveis (Chave/Valor)</param>
        /// <returns>Template com as variávies substituídas</returns>
        public string RenderTemplate(string masterPage, string templateName, IDictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                throw new ArgumentException("O parâmetro \"templateName\" não foi informado", nameof(templateName));
            }

            var name = !string.IsNullOrEmpty(masterPage)
                ? masterPage : templateName;

            var engine = new VelocityEngine();
            var props = new ExtendedProperties();
            props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, _templatesPath);
            engine.Init(props);

            engine.GetTemplate(name, Encoding.UTF8.BodyName);

            var context = new VelocityContext();
            var templateData = data ?? new Dictionary<string, object>();

            foreach (var key in templateData.Keys)
            {
                context.Put(key, templateData[key]);
            }

            if (!string.IsNullOrEmpty(masterPage))
            {
                context.Put("childContent", templateName);
            }
            string retorno;
            using (var writer = new StringWriter())
            {
                engine.MergeTemplate(name, Encoding.UTF8.BodyName, context, writer);
                retorno = writer.GetStringBuilder().ToString();
            }

            return retorno;
        }

        #endregion
    }
}
